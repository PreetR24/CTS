namespace CareSchedule.DTOs
{
    public class ProviderDto
    {
        public int ProviderId { get; set; }
        public string Name { get; set; } = default!;
        public string? Specialty { get; set; }
        public string? Credentials { get; set; }
        public string? ContactInfo { get; set; }
        public string Status { get; set; } = default!;
    }

    public class ProviderCreateDto
    {
        public string Name { get; set; } = default!;
        public string? Specialty { get; set; }
        public string? Credentials { get; set; }
        public string? ContactInfo { get; set; }
    }

    public class ProviderUpdateDto
    {
        public string? Name { get; set; }
        public string? Specialty { get; set; }
        public string? Credentials { get; set; }
        public string? ContactInfo { get; set; }
    }
}
