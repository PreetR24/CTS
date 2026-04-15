namespace CareSchedule.DTOs
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = default!;
        public string VisitType { get; set; } = default!;
        public int DefaultDurationMin { get; set; }
        public int BufferBeforeMin { get; set; }
        public int BufferAfterMin { get; set; }
        public string Status { get; set; } = default!;
    }

    public class ServiceCreateDto
    {
        public string Name { get; set; } = default!;
        public string VisitType { get; set; } = default!;
        public int DefaultDurationMin { get; set; } = 30;
        public int BufferBeforeMin { get; set; } = 0;
        public int BufferAfterMin { get; set; } = 0;
    }

    public class ServiceUpdateDto
    {
        public string? Name { get; set; }
        public string? VisitType { get; set; }
        public int? DefaultDurationMin { get; set; }
        public int? BufferBeforeMin { get; set; }
        public int? BufferAfterMin { get; set; }
    }
}
