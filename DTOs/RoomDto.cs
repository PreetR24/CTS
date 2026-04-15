namespace CareSchedule.DTOs
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = "";
        public string RoomType { get; set; } = "";
        public int SiteId { get; set; }
        public string? AttributesJson { get; set; }
        public string Status { get; set; } = "";
    }

    public class RoomCreateDto
    {
        public string RoomName { get; set; } = "";
        public string RoomType { get; set; } = "General";
        public int SiteId { get; set; }
        public string? AttributesJson { get; set; }
    }

    public class RoomUpdateDto
    {
        public string? RoomName { get; set; }
        public string? RoomType { get; set; }
        public int? SiteId { get; set; }
        public string? AttributesJson { get; set; }
    }

    public class RoomSearchQuery
    {
        public string? RoomName { get; set; }
        public string? Status { get; set; }
        public int? SiteId { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}