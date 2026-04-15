using System;

namespace CareSchedule.DTOs
{
    public class LeaveRequestResponseDto
    {
        public int LeaveId { get; set; }
        public int UserId { get; set; }
        public string LeaveType { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string? Reason { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; } = "";
    }

    public class LeaveSearchDto
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
    }

    public class LeaveImpactResponseDto
    {
        public int ImpactId { get; set; }
        public int LeaveId { get; set; }
        public string ImpactType { get; set; } = "";
        public string? ImpactJson { get; set; }
        public int? ResolvedBy { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Status { get; set; } = "";
    }

    public class CreateLeaveImpactDto
    {
        public int LeaveId { get; set; }
        public string ImpactType { get; set; } = "";
        public string? ImpactJson { get; set; }
    }

    public class ResolveLeaveImpactDto
    {
        public int ResolvedBy { get; set; }
    }

    public class CreateLeaveRequestDto
    {
        public string LeaveType { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string? Reason { get; set; }
    }
}
