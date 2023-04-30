using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class SecurityCompanyNew
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? ConsolCompId { get; set; }
        public string LegalConsolidation { get; set; }
        public int? LegalConsolidationSort { get; set; }
        public string LegalEntity { get; set; }
        public int? LegalEntitySort { get; set; }
        public string ManagementReportingConsolidation { get; set; }
        public int? ManagementReportingConsolidationSort { get; set; }
        public string BusinessStream { get; set; }
        public string BusinessStreamSort { get; set; }
        public DateTime? FinYearStartDate { get; set; }
        public int? CurrentPeriod { get; set; }
        public decimal? Taken { get; set; }
        public DateTime Updated { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
    }
}
