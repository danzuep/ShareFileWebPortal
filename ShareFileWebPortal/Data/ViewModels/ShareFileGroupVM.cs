using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.ShareFile.Models;
using ShareFile.Api.Helpers.Models;

namespace ShareFileWebPortal.Data.ViewModels
{
    public record ShareFileGroupVM
    {
        [ScaffoldColumn(false)]
        public int? Id { get; set; }
        [Display(Name = "Group ID")]
        public string Uid { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string Url { get; set; }
        public ICollection<ShareFileGroupMemberVM> Members { get; set; } = Array.Empty<ShareFileGroupMemberVM>();

        public override string ToString() => String.Format("{0} {1} {2} {3}", Id, Name, IsShared, Uid);
    }

    public static class ShareFileGroupVMExtenions
    {
        public static ShareFileGroupVM ConvertVM(
            this ShareFileApiGroup group, IEnumerable<ShareFileApiGroupMember>? members = null)
        {
            return new ShareFileGroupVM()
            {
                Uid = group.Uid,
                Name = group.Name,
                Url = group.Url,
                IsShared = group.IsShared,
                Members = members?.Count() > 0 ?
                    members.Select(m => m.ConvertVM(group.Name)).ToList() :
                    Array.Empty<ShareFileGroupMemberVM>()
            };
        }

        public static ShareFileGroupVM ConvertVM(
            this ShareFileDbGroup group, IEnumerable<ShareFileGroupMemberVM>? members = null)
        {
            return new ShareFileGroupVM()
            {
                Id = group.Id,
                Uid = group.Uid,
                Name = group.Name,
                Url = group.Url,
                IsShared = group.IsShared.GetValueOrDefault(),
                Members = members?.Count() > 0 ? members.ToList() :
                    Array.Empty<ShareFileGroupMemberVM>()
            };
        }

        public static IEnumerable<ShareFileGroupVM> ConvertVM(this IEnumerable<ShareFileDbGroup>? sfGroups, IEnumerable<ShareFileGroupMemberVM>? members = null)
        {
            return sfGroups == null ? Array.Empty<ShareFileGroupVM>() : sfGroups.Select(g => g.ConvertVM(members));
        }

        public static IEnumerable<ShareFileDbGroup>? ConvertDB(this IEnumerable<ShareFileGroupVM>? sfGroups, IEnumerable<ShareFileGroupMemberVM>? members = null, int? createdBy = null, int? modifiedBy = null)
        {
            return sfGroups == null ? Array.Empty<ShareFileDbGroup>() : sfGroups.Select(g => g.ConvertDB(members, createdBy, modifiedBy));
        }

        public static ShareFileDbGroup ConvertDB(
            this ShareFileGroupVM group, IEnumerable<ShareFileGroupMemberVM>? members = null, int? createdBy = null, int? modifiedBy = null)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            var dbGroup = new ShareFileDbGroup()
                {
                    Uid = group.Uid,
                    Name = group.Name,
                    Url = group.Url,
                    IsShared = group.IsShared,
                    CreatedBy = createdBy,
                    ModifiedBy = modifiedBy
                };
            if (group?.Id > 0)
                dbGroup.Id = group.Id.Value;
            if (members != null)
                dbGroup.ShareFileGroupMembers = members.Select(m => m.ConvertDB(dbGroup)).ToList();
            return dbGroup;
        }

        public static ShareFileDbGroup ConvertDB(
            this ShareFileApiGroup group, ICollection<ShareFileDbGroupMember>? members = null, int? createdBy = null, int? modifiedBy = null)
        {
            var dbGroup = group is null ?
                new ShareFileDbGroup() :
                new ShareFileDbGroup()
                {
                    Uid = group.Uid,
                    Name = group.Name,
                    Url = group.Url,
                    IsShared = group.IsShared,
                    CreatedBy = createdBy,
                    ModifiedBy = modifiedBy
                };
            if (members != null)
                dbGroup.ShareFileGroupMembers = members?.Count() > 0 ? members :
                    Array.Empty<ShareFileDbGroupMember>();
            return dbGroup;
        }
    }
}