using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class ViewTaskTemplate
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string TaskTitle { get; set; } = null!;
        public string? Companies { get; set; }
        public int TaskOwnerId { get; set; }
        public string? TaskOwner { get; set; }
        public int ReportResponsibilityId { get; set; }
        public string? ReportResponsibility { get; set; }
        public int FrequencyId { get; set; }
        public string? Frequency { get; set; }
        public int DueDay { get; set; }
        public string DueTime { get; set; } = null!;
        public int? PublicHolidayOffset { get; set; }
        public bool FixedDate { get; set; }
        public bool IsWorkdayTask { get; set; }
        public int PeriodEnding { get; set; }
        public int? ApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByLogin { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
