using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class RolloverWorksheet
    {
        public int Id { get; set; }
        public int RolloverObjectAccountId { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedById { get; set; }
        public int Order { get; set; }
        public DateTime? Date { get; set; }
        public bool HasAttachments { get; set; }
        public string? FileUid { get; set; }
        public int SaveMethod { get; set; }

        public virtual RolloverObjectAccount RolloverObjectAccount { get; set; } = null!;
    }
}
