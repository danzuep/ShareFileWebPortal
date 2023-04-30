using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Models;
using ShareFile.Api.Helpers.Extensions;
using ShareFile.Api.Helpers.Models;
using System.Text.Encodings.Web;

namespace ShareFile.Api.Helpers
{
    public static class ShareFileApi
    {
        //https://devblogs.microsoft.com/dotnet/configureawait-faq/
        public static string Name => nameof(ShareFileApi);
        public static ILogger Logger { get; set; } = NullLogger.Instance;
        public static ShareFileClient GetDefault() => new ShareFileClient(BaseUrl);
        private const string BaseUrl = "https://secure.sf-api.com/sf/v3/";
        private static string _subdomain = String.Empty;
        private static string _sfAccessToken = string.Empty;

        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public static HttpClient GetHttpClient(string subdomain, string accessToken = "")
        {
            string url = string.Format("https://{0}.sf-api.com/sf/v3/", subdomain);
            var httpClient = new HttpClient() { BaseAddress = new Uri(url) };
            if (!string.IsNullOrEmpty(accessToken))
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "ShareFileApi/1.0");
            //httpClient.Timeout = TimeSpan.FromMinutes(2);
            _subdomain = subdomain;
            _sfAccessToken = accessToken;
            return httpClient;
        }

        public static async Task<Stream> GetStream(this HttpClient client, string request, CancellationToken ct = default)
        {
            Stream stream = Stream.Null;
            //using (var httpClient = client)
            var httpClient = client;
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                string url = string.Format("{0}{1}", httpClient.BaseAddress.AbsoluteUri, request);
                var response = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
                stopwatch.Stop();
                string responseLog = string.Format("{0}<{1}> {2} {3}ms",
                    "GET", nameof(Stream), url, stopwatch.ElapsedMilliseconds);
                if (response.IsSuccessStatusCode)
                    stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                Logger.LogTrace(responseLog);
            }
            return stream;
        }

        public static TOut ChangeType<TIn, TOut>(this TIn value) => (TOut)Convert.ChangeType(value, typeof(TOut));

        public static async Task<T> GetResponseAsync<T>(this HttpClient httpClient, string url, CancellationToken ct = default)
        {
            T result = default(T);
            try
            {
                if (typeof(T) == typeof(string))
                {
                    string text = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                    result = text.ChangeType<string, T>();
                }
                else if (typeof(T) == typeof(Stream))
                {
                    var stream = await httpClient.GetStreamAsync(url).ConfigureAwait(false);
                    result = stream.ChangeType<Stream, T>();
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    var bytes = await httpClient.GetByteArrayAsync(url).ConfigureAwait(false);
                    result = bytes.ChangeType<byte[], T>();
                }
                else
                    result = await httpClient.GetFromJsonAsync<T>(url).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "Failed to parse {0} from JSON.", typeof(T).Name);
            }
            return result;
        }

        public static async Task<T> GetResponseContentAsync<T>(this HttpContent content, CancellationToken ct = default)
        {
            T result = default(T);
            if (typeof(T) == typeof(string))
            {
                string text = await content.ReadAsStringAsync().ConfigureAwait(false);
                result = text.ChangeType<string, T>();
            }
            else if (typeof(T) == typeof(Stream))
            {
                var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
                result = stream.ChangeType<Stream, T>();
            }
            else if (typeof(T) == typeof(byte[]))
            {
                var bytes = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                result = bytes.ChangeType<byte[], T>();
            }
            else
            {
                try
                {
                    result = await content.ReadFromJsonAsync<T>().ConfigureAwait(false);
                }
                catch (JsonException ex)
                {
                    Logger.LogError(ex, "Failed to parse {0} from JSON.", typeof(T).Name);
                }
            }
            return result;
        }

        public static async Task<T> GetResponseContentAsync<T>(this HttpClient client, string request, string type = "GET", int previewLength = 0, CancellationToken ct = default)
        {
            var responseContent = default(T);
            //using (var httpClient = client)
            var httpClient = client;
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                string url = string.Format("{0}{1}", httpClient.BaseAddress.AbsoluteUri, request);
                var response = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
                stopwatch.Stop();
                string responseLog = string.Format("{0}<{1}> {2} {3}ms",
                    type, typeof(T).Name, url, stopwatch.ElapsedMilliseconds);
                if (response.IsSuccessStatusCode)
                {
                    responseContent = await response.Content.GetResponseContentAsync<T>().ConfigureAwait(false);
                    if (typeof(T) == typeof(string) && previewLength > -1)
                    {
                        //var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        //string responseBody = JsonSerializer.Serialize(stream, _jsonOptions);
                        string responseBody = responseContent as string;
                        string responsePreview = responseBody.Length > previewLength ?
                            (previewLength > 0 ? responseBody.Substring(0, previewLength) + "..." : responseBody) : "";
                        responseLog = string.Format("{0} {1}", responseLog, responsePreview);
                    }
                }
                Logger.LogTrace(responseLog);
            }
            return responseContent;
        }

        public static async Task LoadApiText(string subdomain, string accessToken, string url, params object[] args)
        {
            if (args?.Length > 0)
                url = string.Format(url, args);
            using (var httpClient = GetHttpClient(subdomain, accessToken))
            {
                var text = await httpClient.GetResponseContentAsync<string>(url, previewLength: 0).ConfigureAwait(false); //200
            }
        }

        public static async Task<Stream> LoadApiStream(string subdomain, string accessToken, string url, CancellationToken ct = default)
        {
            using (var httpClient = GetHttpClient(subdomain, accessToken))
                return await httpClient.GetStream(url).ConfigureAwait(false);
        }

        public static async Task<T> LoadApiJson<T>(string subdomain, string accessToken, string url, CancellationToken ct = default) where T : class
        {
            using (var httpClient = GetHttpClient(subdomain, accessToken))
                return await httpClient.GetResponseContentAsync<T>(url).ConfigureAwait(false);
        }

        public static async Task<string> LoadApiText(this HttpClient httpClient, string resourcePath, params object[] args)
        {
            if (args?.Length > 0)
                resourcePath = string.Format(resourcePath, args);
            return await httpClient.GetResponseContentAsync<string>(resourcePath, previewLength: 0).ConfigureAwait(false);
        }

        public static async Task<T> LoadApiJson<T>(this HttpClient httpClient, string resourcePath, params object[] args)
        {
            if (args?.Length > 0)
                resourcePath = string.Format(resourcePath, args);
            var groupObj = await httpClient.GetResponseContentAsync<T>(resourcePath).ConfigureAwait(false);
            //var json = JsonSerializer.Serialize(groupObj, _jsonOptions);
            //Logger.LogInformation(json);
            return groupObj;
        }

        public static async Task<IEnumerable<T>> LoadApiValues<T>(this HttpClient httpClient, string resourcePath, params object[] args) where T : IOData
        {
            var groupObj = await httpClient.LoadApiJson<ODataRoot<T>>(resourcePath, args).ConfigureAwait(false);
            return groupObj?.Values ?? Array.Empty<T>();
        }

        public static async Task<IEnumerable<TOut>> LoadShareFileOData<TJson, TOut>(this HttpClient httpClient, string resourcePath, Func<TJson, TOut> asDto, CancellationToken ct = default) where TJson : IOData
        {
            var groupObj = await httpClient.GetResponseContentAsync<ODataRoot<TJson>>(resourcePath, ct: ct).ConfigureAwait(false);
            return groupObj.GetValues(asDto); //ShareFileGroupExtenions.AsGroupDto.Compile
        }

        public static async Task<IEnumerable<ShareFileApiGroup>> LoadShareFileGroups(this HttpClient httpClient, CancellationToken ct = default)
        {
            return await httpClient.LoadShareFileOData<ShareFileGroupChildJson, ShareFileApiGroup>("Groups", x => x.Convert(), ct).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<ShareFileApiGroupMember>> LoadShareFileGroups(this HttpClient httpClient, string groupUid, CancellationToken ct = default)
        {
            string url = string.Format("Groups({0})", groupUid);
            return await httpClient.LoadShareFileOData<ShareFileGroupMemberChildJson, ShareFileApiGroupMember>(url, x => x.Convert(groupUid), ct).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<ShareFileApiGroupMember>> LoadShareFileGroupMembers(this HttpClient httpClient, string groupUid, CancellationToken ct = default)
        {
            string url = string.Format("Groups({0})/Contacts", groupUid);
            return await httpClient.LoadShareFileOData<ShareFileGroupMemberChildJson, ShareFileApiGroupMember>(url, x => x.Convert(groupUid), ct).ConfigureAwait(false);
        }
    }
}
