using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class RolloverAccount
    {
        public RolloverAccount()
        {
            RolloverObjectAccounts = new HashSet<RolloverObjectAccount>();
        }

        public int Id { get; set; }
        public int RolloverId { get; set; }
        public int RecAccountId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int PreparerId { get; set; }
        public int ReviewerId { get; set; }
        public int ApproverId { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public int? Approver2Id { get; set; }
        public string? DocumentFolderUrl { get; set; }
        public int? Approver3Id { get; set; }
        public bool? InternallyReviewed { get; set; }
        public DateTime? InternallyReviewedOn { get; set; }
        public int? InternallyReviewedById { get; set; }
        public bool DocsCopied { get; set; }
        public string? FileUid { get; set; }
        public int SaveMethod { get; set; }

        public virtual ICollection<RolloverObjectAccount> RolloverObjectAccounts { get; set; }
    }
}
