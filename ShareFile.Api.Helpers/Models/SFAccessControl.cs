using ShareFile.Api.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareFile.Api.Helpers.Models
{
    public class SFAccessControl : OData
    {
        public OData Principal { get; set; }
        public bool CanUpload { get; set; }
        public bool CanDownload { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManagePermissions { get; set; }
        public bool NotifyOnUpload { get; set; }
        public bool NotifyOnDownload { get; set; }
        public bool IsOwner { get; set; }
    }

    public class SFItemAccessControl
    {
        public Folder Folder { get; set; }
        public IEnumerable<AccessControl> AccessControls { get; set; }
    }

    public class SFUserItemSecurity
    {
        public string PrincipalUid { get; set; }
        public string FolderUid { get; set; }
        public string FolderPath { get; set; }
        public string FolderName { get; set; }
        public bool CanView { get; set; }
        public bool CanDownload { get; set; }
        public bool CanUpload { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManagePermissions { get; set; }
        public bool NotifyOnDownload { get; set; }
        public bool NotifyOnUpload { get; set; }
        public bool IsOwner { get; set; }
        public bool IsGroup => string.IsNullOrEmpty(PrincipalEmail);
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public string FolderPathCombined => Path.Combine(FolderPath, FolderName).Replace('\\', '/');
        public int ItemCount { get; set; }
        public int ItemSizeInKB { get; set; }

        public override string ToString()
        {
            return $"{PrincipalUid} {FolderUid} '{FolderPathCombined}' {CanDownload} {CanUpload} {CanDelete} {CanManagePermissions} {NotifyOnDownload} {NotifyOnUpload} {CanView} {IsOwner} {PrincipalName}";
        }
    }

    public class SFUserSummary
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
