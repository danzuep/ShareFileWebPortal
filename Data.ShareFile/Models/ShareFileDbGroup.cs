using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.ShareFile.Models
{
    public record ShareFileDbGroup : EntityIdBase
    {
        public ShareFileDbGroup()
        {
            ShareFileGroupMembers = new HashSet<ShareFileDbGroupMember>();
        }

        [StringLength(100)]
        public string Name { get; set; }

        public bool? IsShared { get; set; }

        [StringLength(1000)]
        public string Url { get; set; }

        [Display(Name = "Group Members")]
        public virtual ICollection<ShareFileDbGroupMember> ShareFileGroupMembers { get; set; }
    }
}
