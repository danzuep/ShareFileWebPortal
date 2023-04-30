using System;
using System.Collections.Generic;

namespace Data.WebPortal.Models
{
    public partial class ViewTimetableTask
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public bool? IsEnabled { get; set; }
        public int TaskTypeId { get; set; }
        public string TaskType { get; set; } = null!;
        public int TimetableTaskOutcome { get; set; }
        public string? TimetableTaskStatus { get; set; }
        public string TimetableTaskTitle { get; set; } = null!;
        public int CompanyId { get; set; }
        public int TaskOwnerId { get; set; }
        public string? TaskOwner { get; set; }
        public int ReportResponsibilityId { get; set; }
        public string? ReportResponsibility { get; set; }
        public int? TaskTemplateOwnerId { get; set; }
        public int? TaskTemplateReportResponsibilityId { get; set; }
        public string? TaskFrequency { get; set; }
        public int DueDay { get; set; }
        public string? DueTime { get; set; }
        public DateTime DueDate { get; set; }
        public int? PublicHolidayOffset { get; set; }
        public bool FixedDate { get; set; }
        public bool IsWorkdayTask { get; set; }
        public int TimetableMonthRolloverId { get; set; }
        public string? TimetableModule { get; set; }
        public int ApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public int DepartmentId { get; set; }
        public string? Department { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByLogin { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
