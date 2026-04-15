using System;

namespace CareSchedule.DTOs
{
    public class CreateChargeRefDto
    {
        public int AppointmentId { get; set; }
        public int ServiceId { get; set; }
        public int ProviderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }

    public class ChargeRefResponseDto
    {
        public int ChargeRefId { get; set; }
        public int AppointmentId { get; set; }
        public int ServiceId { get; set; }
        public int ProviderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class ChargeSearchDto
    {
        public int? AppointmentId { get; set; }
        public int? ProviderId { get; set; }
        public string? Status { get; set; }
    }
}
