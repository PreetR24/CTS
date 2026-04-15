namespace CareSchedule.DTOs
{
    public class SystemConfigDto
    {
        public int ConfigId { get; set; }
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public string Scope { get; set; } = "";
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; } = "";
    }

    public class SystemConfigCreateDto
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public string Scope { get; set; } = "Global";
        public int? UpdatedBy { get; set; }
    }

    public class SystemConfigUpdateDto
    {
        public string? Value { get; set; }
        public string? Scope { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class SystemConfigSearchQuery
    {
        public string? Key { get; set; }
        public string? Scope { get; set; }
        public string? SortBy { get; set; } = "updateddate";
        public string? SortDir { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}