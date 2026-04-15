namespace CareSchedule.DTOs
{
    public class HolidayDto
    {
        public int HolidayId { get; set; }
        public int SiteId { get; set; }
        public string Date { get; set; } = "";
        public string? Description { get; set; }
        public string Status { get; set; } = "";
    }

    public class HolidayCreateDto
    {
        public int SiteId { get; set; }
        public string Date { get; set; } = "";
        public string? Description { get; set; }
    }

    public class HolidayUpdateDto
    {
        public int? SiteId { get; set; }
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }

    public class HolidaySearchQuery
    {
        public int? SiteId { get; set; }
        public string? Date { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}