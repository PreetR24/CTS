namespace CareSchedule.DTOs
{
    public class ProviderServiceDto
    {
        public int Psid { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = default!;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = default!;
        public int? CustomDurationMin { get; set; }
        public int? CustomBufferBeforeMin { get; set; }
        public int? CustomBufferAfterMin { get; set; }
        public string Status { get; set; } = default!;
    }

    public class ProviderServiceCreateDto
    {
        public int ProviderId { get; set; }
        public int ServiceId { get; set; }
        public int? CustomDurationMin { get; set; }
        public int? CustomBufferBeforeMin { get; set; }
        public int? CustomBufferAfterMin { get; set; }
    }
}
