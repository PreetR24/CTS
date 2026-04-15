using System;

namespace CareSchedule.DTOs
{
    public class ResourceHoldCreateDto
    {
        public string ResourceType { get; set; } = "";
        public int ResourceId { get; set; }
        public int SiteId { get; set; }
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public string? Reason { get; set; }
    }

    public class ResourceHoldUpdateDto
    {
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
    }

    public class ResourceHoldResponseDto
    {
        public int HoldId { get; set; }
        public string ResourceType { get; set; } = "";
        public int ResourceId { get; set; }
        public int SiteId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "";
    }
}