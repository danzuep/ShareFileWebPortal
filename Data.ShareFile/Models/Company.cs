using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class Company
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public int CompanyId { get; set; }
        public bool RecTaskEnabled { get; set; }
        public int? DefaultPreparerId { get; set; }
        public int? DefaultReviewerId { get; set; }
        public int? DefaultApproverId { get; set; }
        public int? DefaultApprover2Id { get; set; }
        public int? DefaultApprover3Id { get; set; }

        public virtual ApplicationUser DefaultApprover { get; set; }
        public virtual ApplicationUser DefaultApprover2 { get; set; }
        public virtual ApplicationUser DefaultApprover3 { get; set; }
        public virtual ApplicationUser DefaultPreparer { get; set; }
        public virtual ApplicationUser DefaultReviewer { get; set; }
    }
}
