using System;

namespace CareSchedule.DTOs
{
    public class CalendarEventResponseDto
    {
        public int EventId { get; set; }
        public string EntityType { get; set; } = "";
        public int EntityId { get; set; }
        public int? ProviderId { get; set; }
        public int SiteId { get; set; }
        public int? RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "";
    }

    public class CalendarSearchDto
    {
        public int? ProviderId { get; set; }
        public int? SiteId { get; set; }
        public string? Date { get; set; }
    }
}
