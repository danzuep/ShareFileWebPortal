using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class RecAllocationView
    {
        public int Id { get; set; }
        public bool? Selected { get; set; }
        public int CompanyId { get; set; }
        public int? RecAccountId { get; set; }
        public string? RecAccountTitle { get; set; }
        public string? RecAccountObjectAccount { get; set; }
        public int? RolloverId { get; set; }
        public DateTime? RolloverDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedById { get; set; }
        public int PreparerId { get; set; }
        public int ReviewerId { get; set; }
        public int ApproverId { get; set; }
        public int? Approver2Id { get; set; }
        public int? Approver3Id { get; set; }
        public string? Preparer { get; set; }
        public string? Reviewer { get; set; }
        public string? Approver { get; set; }
        public string? Approver2 { get; set; }
        public string? Approver3 { get; set; }
    }
}
