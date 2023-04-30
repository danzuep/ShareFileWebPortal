using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.ShareFile.Models;
using ShareFile.Api.Helpers.Models;

namespace ShareFileWebPortal.Data.ViewModels
{
    public record ShareFileGroupMemberVM
    {
        [ScaffoldColumn(false)]
        public int? Id { get; set; }
        public string GroupUid { get; set; }
        [Display(Name = "Group")]
        public string GroupName { get; set; }
        [Display(Name = "ID")]
        public string MemberUid { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Display(Name = "URL")]
        public string Url { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsDisabled { get; set; }
        public DateTime? LastAnyLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public static class ShareFileGroupMemberExtenions
    {
        public static ShareFileGroupMemberVM ConvertVM(
            this ShareFileApiGroupMember member, string groupName)
        {
            return new ShareFileGroupMemberVM()
            {
                GroupName = groupName,
                GroupUid = member.GroupUid,
                MemberUid = member.MemberUid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.FullName,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate
            };
        }

        public static ShareFileGroupMemberVM ConvertVM(
            this ShareFileDbGroupMember member)
        {
            return new ShareFileGroupMemberVM()
            {
                Id = member.Id,
                GroupName = member.Group.Name,
                GroupUid = member.Group.Uid,
                MemberUid = member.Uid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.FullName,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate
            };
        }

        public static IEnumerable<ShareFileGroupMemberVM> ConvertVM(this IEnumerable<ShareFileDbGroupMember>? sfGroups)
        {
            return sfGroups == null ? Array.Empty<ShareFileGroupMemberVM>() : sfGroups.Select(g => g.ConvertVM());
        }

        public static IEnumerable<ShareFileDbGroupMember>? ConvertDB(this IEnumerable<ShareFileGroupMemberVM>? sfGroups, ShareFileDbGroup? group = null)
        {
            return sfGroups == null ? Array.Empty<ShareFileDbGroupMember>() : sfGroups.Select(g => g.ConvertDB(group));
        }

        public static ShareFileDbGroupMember ConvertDB(
            this ShareFileGroupMemberVM member, ShareFileDbGroup? group)
        {
            return new ShareFileDbGroupMember()
            {
                Uid = member.MemberUid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.FullName,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate,
                Group = group
            };
        }

        public static ShareFileDbGroupMember ConvertDB(
            this ShareFileApiGroupMember member, ShareFileDbGroup? group)
        {
            return new ShareFileDbGroupMember()
            {
                Uid = member.MemberUid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.FullName,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate,
                Group = group
            };
        }
    }
}