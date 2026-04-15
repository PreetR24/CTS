namespace CareSchedule.DTOs
{
    // ---------- Common ----------
    public class IdResponseDto
    {
        public int Id{ get; set; }
    }

    // ---------- Availability Template ----------
    public class CreateAvailabilityTemplateRequestDto
    {
        public int ProviderId{ get; set; }
        public int SiteId{ get; set; }
        public int DayOfWeek { get; set; } // 0-6 (Sun-Sat)
        public string StartTime { get; set; } = string.Empty; // "09:00"
        public string EndTime { get; set; } = string.Empty;   // "13:00"
        public int SlotDurationMin { get; set; }
        public string Status { get; set; } = "Active"; // Active/Inactive
    }

    public class UpdateAvailabilityTemplateRequestDto : CreateAvailabilityTemplateRequestDto
    {
        public int TemplateId{ get; set; }
    }

    public class AvailabilityTemplateResponseDto
    {
        public int TemplateId{ get; set; }
        public int ProviderId{ get; set; }
        public int SiteId{ get; set; }
        public int DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int SlotDurationMin { get; set; }
        public string Status { get; set; } = "Active";
    }

    // ---------- Availability Block ----------
    public class CreateAvailabilityBlockRequestDto
    {
        public int ProviderId{ get; set; }
        public int SiteId{ get; set; }
        public string Date { get; set; } = string.Empty;      // "2026-03-08"
        public string StartTime { get; set; } = string.Empty; // "09:00"
        public string EndTime { get; set; } = string.Empty;   // "11:00"
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active/Cancelled
    }

    public class AvailabilityBlockResponseDto
    {
        public int BlockId{ get; set; }
        public int ProviderId{ get; set; }
        public int SiteId{ get; set; }
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    // ---------- Slots (Read-only API) ----------
    public class SlotSearchRequestDto
    {
        public int ProviderId{ get; set; }
        public int ServiceId{ get; set; }
        public int SiteId{ get; set; }
        public string Date { get; set; } = string.Empty; // "2026-03-12"
    }

    public class SlotResponseDto
    {
        public int PubSlotId{ get; set; }
        public int ProviderId{ get; set; }
        public int ServiceId{ get; set; }
        public int SiteId{ get; set; }
        public string SlotDate { get; set; } = string.Empty; // "2026-03-12"
        public string StartTime { get; set; } = string.Empty; // "10:00"
        public string EndTime { get; set; } = string.Empty;   // "10:30"
        public string Status { get; set; } = "Open";          // Open/Held/Closed
    }

    // ---------- Blackout ----------
    public class CreateBlackoutRequestDto
    {
        public int SiteId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class BlackoutResponseDto
    {
        public int BlackoutId { get; set; }
        public int SiteId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string Status { get; set; } = "Active";
    }

    // ---------- Slot Generation (internal trigger) ----------
    public class GenerateSlotsRequestDto
    {
        public int SiteId{ get; set; }
        public int Days { get; set; } = 14; // default horizon
    }

    public class GenerateSlotsResponseDto
    {
        public int InsertedCount { get; set; }
        public int SkippedExistingCount { get; set; }
    }
}