using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class RolloverAccountView
    {
        public int Id { get; set; }
        public int RecAccountId { get; set; }
        public int RolloverId { get; set; }
        public DateTime? RolloverDate { get; set; }
        public int? Frequency { get; set; }
        public int? CompanyId { get; set; }
        public string? RecAccountObjectAccount { get; set; }
        public string? RecAccountTitle { get; set; }
        public int? Status { get; set; }
        public decimal? LastMonthBalance { get; set; }
        public decimal? Movement { get; set; }
        public decimal? CurrentMonthBalance { get; set; }
        public int PreparerId { get; set; }
        public string? PreparerFullName { get; set; }
        public int ReviewerId { get; set; }
        public string? ReviewerFullName { get; set; }
        public int ApproverId { get; set; }
        public string? ApproverFullName { get; set; }
        public int? Approver2Id { get; set; }
        public string? Approver2FullName { get; set; }
        public int? Approver3Id { get; set; }
        public string? Approver3FullName { get; set; }
        public int? DueDay { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public string? CreatedByFullName { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedById { get; set; }
        public string? ModifiedByFullName { get; set; }
        public bool? RecEnabled { get; set; }
        public bool? RecTaskEnabled { get; set; }
        public string? DocumentFolderUrl { get; set; }
    }
}
