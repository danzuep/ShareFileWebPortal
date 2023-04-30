using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShareFile.Api.Helpers.Models
{
    public class SFUser : SFContactBase
    {
        public bool IsAdministrator { get; set; }
        public bool CanCreateFolders { get; set; }
        public bool CanUseFileBox { get; set; }
        public bool CanManageUsers { get; set; }
        public bool IsVirtualClient { get; set; }
        public int DiskSpace { get; set; }
        public int Bandwidth { get; set; }
        public int TotalSharedFiles { get; set; }
        public int Contacted { get; set; }
        public string FullName { get; set; }
        public string ReferredBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string FullNameShort { get; set; }
        public ICollection<string> Emails { get; set; }
        public ICollection<EmailAddress> EmailAddresses { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<string> Roles { get; set; }
        public string AffiliatedPartnerUserId { get; set; }
        public bool IsBillingContact { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
    }

    public class SFContact : SFContactBase
    {
        public bool IsDisabled { get; set; }
    }

    public class SFContactBase : OData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime LastAnyLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EmailAddress
    {
        public string Email { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsPrimary { get; set; }

        [JsonPropertyName("odata.type")]
        public string Type { get; set; }
    }
}
