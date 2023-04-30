using System;
using System.Net;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Security.Authentication.OAuth2;
using ShareFile.Api.Helpers.Extensions;

namespace ShareFile.Api.Helpers.Models
{
    public class ShareFileApiOptions
    {
        public string BaseUrl { get; set; } = "https://secure.sf-api.com/sf/v3/";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string SubDomain { get; set; }
        public string ControlPlane { get; set; } = "sharefile.com";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public NetworkCredential Credential => new NetworkCredential(UserName, Password, SubDomain);

        private Uri _uri;
        public Uri Uri
        {
            get
            {
                if (_uri == null)
                    _uri = new Uri(GetUrl(SubDomain, ControlPlane));
                return _uri;
            }
        }

        internal static string GetUrl(string subdomain, string domain = "sharefile.com",
            string subdirectory = "home/shared/", string itemUid = "", bool isHttps = true)
        {
            string scheme = isHttps ? "https" : "http";
            return String.Format("{0}://{1}.{2}/{3}{4}", scheme, subdomain, domain, subdirectory, itemUid);
        }

        private ShareFileClient _shareFileClient;
        public ShareFileClient AuthenticatedClient => GetShareFileClient();

        private OAuthService _oAuthService;
        public OAuthService OAuthService
        {
            get
            {
                if (_oAuthService == null)
                    _oAuthService = new OAuthService(
                        AuthenticatedClient, ClientId, ClientSecret);
                return _oAuthService;
            }
        }

        private OAuthToken _oAuthToken;
        public OAuthToken OAuthToken
        {
            get
            {
                if (_oAuthToken == null)
                {
                    _oAuthToken = OAuthService.GetPasswordGrantRequestQuery(
                        UserName, Password, SubDomain, ControlPlane).Execute();
                    _shareFileClient.AddOAuthCredentials(OAuthToken);
                    _shareFileClient.BaseUri = OAuthToken.GetUri();
                }
                return _oAuthToken;
            }
        }

        private ShareFileClient GetShareFileClient()
        {
            if (_shareFileClient == null)
            {
                if (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret))
                    throw new ArgumentNullException("You must provide oauthClientId and oauthClientSecret");
                else if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(SubDomain))
                    throw new ArgumentNullException("You must provide username, password and subdomain");

                _shareFileClient = ShareFileApi.GetDefault();
                _oAuthService = new OAuthService(
                    _shareFileClient, ClientId, ClientSecret);
                _oAuthToken = _oAuthService.GetPasswordGrantRequestQuery(
                    UserName, Password, SubDomain, ControlPlane).Execute();
                _shareFileClient.AddOAuthCredentials(_oAuthToken);
                _shareFileClient.BaseUri = _oAuthToken.GetUri();
            }
            return _shareFileClient;
        }

        public OAuthService InitialiseShareFileSevice
        {
            get
            {
                var sfClient = ShareFileApi.GetDefault();
                var sfService = sfClient.GetOAuthService(ClientId, ClientSecret);
                return sfService;
            }
        }

        public IShareFileClient InitialiseShareFileClient => InitialiseShareFileSevice.ShareFileClient;
    }
}
