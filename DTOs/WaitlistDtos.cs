using System;

namespace CareSchedule.DTOs
{
    public class CreateWaitlistRequestDto
    {
        public int SiteId { get; set; }
        public int ProviderId { get; set; }
        public int ServiceId { get; set; }
        public int PatientId { get; set; }
        public string Priority { get; set; } = "Normal";
        public string? RequestedDate { get; set; }
    }

    public class WaitlistResponseDto
    {
        public int WaitId { get; set; }
        public int SiteId { get; set; }
        public int ProviderId { get; set; }
        public int ServiceId { get; set; }
        public int PatientId { get; set; }
        public string Priority { get; set; } = "";
        public string RequestedDate { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class WaitlistSearchDto
    {
        public int? SiteId { get; set; }
        public int? ProviderId { get; set; }
        public int? ServiceId { get; set; }
        public int? PatientId { get; set; }
        public string? Status { get; set; }
    }

    public class FillWaitlistRequestDto
    {
        public string? BookingChannel { get; set; }
    }
}
