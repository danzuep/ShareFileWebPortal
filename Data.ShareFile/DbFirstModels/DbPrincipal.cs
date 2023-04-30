using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Data.ShareFile.Models;

namespace Data.ShareFile.DbFirstModels
{
    [Index("Uid", Name = "IX_Group_Uid")]
    [Index("FirstName", "LastName", Name = "IX_Principals_FirstName_LastName")]
    [Index("GroupId", Name = "IX_Principals_GroupId")]
    [Index("OwnerId", Name = "IX_Principals_OwnerId")]
    [Index("Uid", Name = "IX_Principals_Uid")]
    public record DbPrincipal : EntityUidBase
    {
        public int Discriminator { get; set; }
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(512)]
        public string Email { get; set; }
        [StringLength(512)]
        public string Username { get; set; }
        [StringLength(128)]
        public string Domain { get; set; }
        [StringLength(36)]
        [Unicode(false)]
        public string GroupId { get; set; }
        [StringLength(36)]
        [Unicode(false)]
        public string OwnerId { get; set; }
        public bool? IsShared { get; set; }
        public int? MemberCount { get; set; }
        [StringLength(256)]
        public string FirstName { get; set; }
        [StringLength(256)]
        public string LastName { get; set; }
        [StringLength(512)]
        public string Company { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsDisabled { get; set; }
        public int? TotalSharedFiles { get; set; }
        public DateTime? LastAnyLogin { get; set; }
        [StringLength(514)]
        public string FullName { get; set; }
        //public virtual ICollection<ShareFileDbGroup> Groups { get; set; }
    }
}
