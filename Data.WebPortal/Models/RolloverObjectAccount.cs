using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class RolloverObjectAccount
    {
        public RolloverObjectAccount()
        {
            RolloverWorksheets = new HashSet<RolloverWorksheet>();
        }

        public int Id { get; set; }
        public int ObjectAccountId { get; set; }
        public int RolloverAccountId { get; set; }
        public string? ProjectOrSubLedger { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public string? FileUid { get; set; }
        public int SaveMethod { get; set; }

        public virtual RolloverAccount RolloverAccount { get; set; } = null!;
        public virtual ICollection<RolloverWorksheet> RolloverWorksheets { get; set; }
    }
}
