using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class SecurityUserCompany
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedById { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual ApplicationUser ModifiedBy { get; set; }
    }
}
