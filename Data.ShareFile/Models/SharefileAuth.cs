using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class SharefileAuth
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public string Appcp { get; set; }
        public string Apicp { get; set; }
        public string Subdomain { get; set; }
        public int ExpiresIn { get; set; }
        public string SessionId { get; set; }
        public DateTime Exipiry { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedById { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual ApplicationUser ModifiedBy { get; set; }
    }
}
