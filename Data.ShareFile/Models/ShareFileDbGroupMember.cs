using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using ShareFile.Api.Helpers.Models;

namespace Data.ShareFile.Models
{
    public record ShareFileDbGroupMember : EntityIdBase
    {
        //[Display(Name = "Group ID")]
        //[ForeignKey("Group")]
        //[ScaffoldColumn(false)]
        public int? GroupId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        public bool? IsConfirmed { get; set; }

        public bool? IsDisabled { get; set; }

        public DateTime? LastAnyLogin { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(200)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(1000)]
        public string Url { get; set; }

        public virtual ShareFileDbGroup Group { get; set; } = null!;
    }

    public static class ShareFileGroupMemberExtenions
    {
        public static readonly Expression<Func<ShareFileApiGroupMember, ShareFileDbGroupMember>> AsGroupMemberDto =
            member => new ShareFileDbGroupMember
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
            };

        public static IQueryable<ShareFileDbGroupMember> GetGroupMembers(IQueryable<ShareFileApiGroupMember> groups)
        {
            return groups.Select(AsGroupMemberDto);
        }

        public static ShareFileDbGroupMember Convert(
            this ShareFileApiGroupMember member, ShareFileDbGroup group)
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
