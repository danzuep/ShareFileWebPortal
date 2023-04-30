using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ShareFile.Api.Helpers.Models
{
    public class ShareFileApiGroupMember
    {
        public string GroupUid { get; set; }
        public string MemberUid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? LastAnyLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }

    public class ShareFileGroupMemberChildJson : OData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAnyLogin { get; set; }
    }

    public static class ShareFileGroupMemberExtenions
    {
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/
        public static readonly Expression<Func<ShareFileGroupMemberChildJson, ShareFileApiGroupMember>> AsGroupDto =
            member => new ShareFileApiGroupMember
            {
                MemberUid = member.Uid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.Name,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate,
            };

        public static IQueryable<ShareFileApiGroupMember> GetGroups(this IQueryable<ShareFileGroupMemberChildJson> groups)
        {
            return groups.Select(AsGroupDto);
        }

        public static ShareFileApiGroupMember Convert(
            this ShareFileGroupMemberChildJson member, string groupUid)
        {
            return new ShareFileApiGroupMember()
            {
                GroupUid = groupUid,
                MemberUid = member.Uid,
                Company = member.Company,
                FirstName = member.FirstName,
                LastName = member.LastName,
                FullName = member.Name,
                Email = member.Email,
                Url = member.Url,
                IsConfirmed = member.IsConfirmed,
                IsDisabled = member.IsDisabled,
                LastAnyLogin = member.LastAnyLogin,
                CreatedDate = member.CreatedDate,
            };
        }
    }
}