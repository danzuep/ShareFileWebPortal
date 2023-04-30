using System;

namespace ShareFile.Api.Helpers.Models
{
    public class SFUserSecurity : OData
    {
        public bool IsDisabled { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockExpires { get; set; }
        public DateTime LastWebAppLogin { get; set; }
        public DateTime LastAnyLogin { get; set; }
        public DateTime FirstAnyLogin { get; set; }
        public string UserIPRestrictions { get; set; }
        public DateTime DisableLoginBefore { get; set; }
        public DateTime DisableLoginAfter { get; set; }
        public bool ForcePasswordChange { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public string UsernameShort { get; set; }
        public DateTime LastFailedLogin { get; set; }
        public string LastFailedLoginIP { get; set; }
        public int FailedLoginCount { get; set; }
        public string UserAuthenticationType { get; set; }
    }
}
