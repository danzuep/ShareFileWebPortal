using System;
using System.Collections.Generic;
using System.Text;

namespace ShareFile.Api.Helpers.Models
{
    public class Rootobject : OData
    {
        public SFSharedFolder[] value { get; set; }
    }

    public class SFSharedFolder : OData
    {
        public int FileCount { get; set; }
        public SFFolderInfo Info { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public OData Parent { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ProgenyEditDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
        public int DiskSpaceLimit { get; set; }
        public bool IsHidden { get; set; }
        public int BandwidthLimitInMB { get; set; }
        public int FileSizeInKB { get; set; }
        public string Path { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public int ExpirationDays { get; set; }
        public long FileSizeBytes { get; set; }
        public bool HasPendingDeletion { get; set; }
        public string AssociatedFolderTemplateID { get; set; }
        public bool IsTemplateOwned { get; set; }
        public bool HasPermissionInfo { get; set; }
        public int State { get; set; }
        public string StreamID { get; set; }
        public string CreatorNameShort { get; set; }
        public bool HasPendingAsyncOp { get; set; }
    }

    public class SFFolderInfo : OData
    {
        public bool HasVroot { get; set; }
        public bool IsSystemRoot { get; set; }
        public bool IsAccountRoot { get; set; }
        public bool IsVRoot { get; set; }
        public bool IsMyFolders { get; set; }
        public bool IsAHomeFolder { get; set; }
        public bool IsMyHomeFolder { get; set; }
        public bool IsAStartFolder { get; set; }
        public bool IsSharedFolder { get; set; }
        public bool IsPassthrough { get; set; }
        public bool CanAddFolder { get; set; }
        public bool CanAddNode { get; set; }
        public bool CanView { get; set; }
        public bool CanDownload { get; set; }
        public bool CanUpload { get; set; }
        public bool CanSend { get; set; }
        public bool CanDeleteCurrentItem { get; set; }
        public bool CanDeleteChildItems { get; set; }
        public bool CanManagePermissions { get; set; }
        public bool CanCreateOfficeDocuments { get; set; }
        public string FolderPayID { get; set; }
        public bool ShowFolderPayBuyButton { get; set; }
    }
}
