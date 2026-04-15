using System;

namespace CareSchedule.DTOs
{
    // --------- ShiftTemplate ---------
    public class CreateShiftTemplateDto
    {
        public string Name { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public int BreakMinutes { get; set; }
        public string Role { get; set; } = "";
        public int SiteId { get; set; }
    }

    public class UpdateShiftTemplateDto
    {
        public string? Name { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? BreakMinutes { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }

    public class ShiftTemplateResponseDto
    {
        public int ShiftTemplateId { get; set; }
        public string Name { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public int BreakMinutes { get; set; }
        public string Role { get; set; } = "";
        public int SiteId { get; set; }
        public string Status { get; set; } = "";
    }

    // --------- Roster ---------
    public class CreateRosterDto
    {
        public int SiteId { get; set; }
        public string? Department { get; set; }
        public string PeriodStart { get; set; } = "";
        public string PeriodEnd { get; set; } = "";
    }

    public class RosterResponseDto
    {
        public int RosterId { get; set; }
        public int SiteId { get; set; }
        public string? Department { get; set; }
        public string PeriodStart { get; set; } = "";
        public string PeriodEnd { get; set; } = "";
        public int? PublishedBy { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Status { get; set; } = "";
    }

    public class PublishRosterDto
    {
        public int PublishedBy { get; set; }
    }

    public class UpdateRosterDto
    {
        public string? Department { get; set; }
        public string? PeriodStart { get; set; }
        public string? PeriodEnd { get; set; }
        public string? Status { get; set; }
    }

    // --------- RosterAssignment ---------
    public class CreateRosterAssignmentDto
    {
        public int RosterId { get; set; }
        public int UserId { get; set; }
        public int ShiftTemplateId { get; set; }
        public string Date { get; set; } = "";
        public string? Role { get; set; }
    }

    public class RosterAssignmentResponseDto
    {
        public int AssignmentId { get; set; }
        public int RosterId { get; set; }
        public int UserId { get; set; }
        public int ShiftTemplateId { get; set; }
        public string Date { get; set; } = "";
        public string? Role { get; set; }
        public string Status { get; set; } = "";
    }

    public class SwapAssignmentDto
    {
        public int? NewUserId { get; set; }
        public int? NewShiftTemplateId { get; set; }
    }

    public class RosterAssignmentSearchDto
    {
        public int? SiteId { get; set; }
        public int? UserId { get; set; }
        public string? Date { get; set; }
        public string? Status { get; set; }
    }

    // --------- Search DTOs ---------
    public class ShiftTemplateSearchDto
    {
        public int? SiteId { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }

    public class RosterSearchDto
    {
        public int? SiteId { get; set; }
        public string? Status { get; set; }
    }

    public class OnCallSearchDto
    {
        public int? SiteId { get; set; }
        public string? Date { get; set; }
    }

    // --------- OnCallCoverage ---------
    public class CreateOnCallDto
    {
        public int SiteId { get; set; }
        public string? Department { get; set; }
        public string Date { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public int PrimaryUserId { get; set; }
        public int? BackupUserId { get; set; }
    }

    public class UpdateOnCallDto
    {
        public string? Department { get; set; }
        public string? Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? PrimaryUserId { get; set; }
        public int? BackupUserId { get; set; }
        public string? Status { get; set; }
    }

    public class OnCallResponseDto
    {
        public int OnCallId { get; set; }
        public int SiteId { get; set; }
        public string? Department { get; set; }
        public string Date { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public int PrimaryUserId { get; set; }
        public int? BackupUserId { get; set; }
        public string Status { get; set; } = "";
    }
}
