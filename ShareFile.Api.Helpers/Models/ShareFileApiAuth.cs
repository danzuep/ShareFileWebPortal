using System;

namespace ShareFile.Api.Helpers.Models
{
    public class ShareFileApiAuth
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public string AppCp { get; set; }
        public string ApiCp { get; set; }
        public string SubDomain { get; set; }
        public long ExpiresIn { get; set; }

        public string SessionId { get; set; }
        public DateTime Exipiry { get; set; }
    }
}