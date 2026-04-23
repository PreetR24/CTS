namespace CareSchedule.DTOs
{
    public class AuditLogDto
    {
        public int AuditId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = "";
        public string Resource { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string? Metadata { get; set; }
    }

    public class AuditLogCreateDto
    {
        public int? UserId { get; set; }
        public string Action { get; set; } = "";
        public string Resource { get; set; } = "";
        public string? Timestamp { get; set; }
        public string? Metadata { get; set; }
    }

    public class AuditLogSearchQuery
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? Resource { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortBy { get; set; } = "timestamp";
        public string? SortDir { get; set; } = "desc";
    }
}