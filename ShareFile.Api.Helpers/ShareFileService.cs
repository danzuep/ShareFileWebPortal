using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ShareFile.Api.Helpers.Extensions;
using ShareFile.Api.Helpers.Models;
using Common.Helpers;

namespace ShareFile.Api.Helpers
{
    public class ShareFileService : IDisposable
    {
        private static string _name = nameof(ShareFileService);
        private readonly ILogger _logger = LogProvider.GetLogger(_name);
        private readonly ShareFileApiOptions _sfOptions;
        private string _sfAccessToken = string.Empty;
        private HttpClient _httpClient;
        private readonly Task _initialisation;

        public ShareFileService(ILogger<ShareFileService> logger, IConfiguration config)
        {
            _logger = logger;
            _sfOptions = config.GetRequiredSection("ShareFile").Get<ShareFileApiOptions>();
            _logger.LogInformation("{name} started at {time}", _name, DateTimeOffset.Now.ToString("G"));
            _initialisation = InitialiseAsync();
        }

        private async Task InitialiseAsync(CancellationToken ct = default)
        {
            if (_sfOptions is null)
                System.Diagnostics.Debugger.Break();
            if (string.IsNullOrEmpty(_sfAccessToken) && _sfOptions != null)
            {
                ShareFileApi.Logger = LogProvider.GetLogger(ShareFileApi.Name);
                var sfService = _sfOptions.InitialiseShareFileSevice;
                var sfToken = await sfService.StartSessionAsync(_sfOptions.Credential);
                _sfAccessToken = sfToken.AccessToken;
                _httpClient = ShareFileApi.GetHttpClient(_sfOptions.SubDomain, _sfAccessToken);
            }
        }

        //public async Task<IEnumerable<T>> GetFromShareFileApiAsync<T, TJson>(string url, CancellationToken ct = default) where TJson : ODataRoot<TJson>
        //{
        //    await _initialisation;
        //    var groupObj = await _httpClient.GetFromJsonAsync<TJson>(url).ConfigureAwait(false);
        //    return groupObj.Values.Select(x => x.Convert());
        //}

        public async Task<IEnumerable<ShareFileApiGroup>> GetShareFileGroupsAsync(CancellationToken ct = default)
        {
            await _initialisation;
            return await _httpClient.LoadShareFileGroups(ct);
        }

        public async Task<IEnumerable<ShareFileApiGroupMember>> GetShareFileApiGroupMembersAsync(string groupUid, CancellationToken ct = default)
        {
            await _initialisation;
            return await _httpClient.LoadShareFileGroupMembers(groupUid, ct);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
