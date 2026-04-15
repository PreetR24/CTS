namespace CareSchedule.DTOs
{
    public class SiteDto
    {
        public int SiteId { get; set; }
        public string Name { get; set; } = "";
        public string? AddressJson { get; set; }
        public string Timezone { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class SiteCreateDto
    {
        public string Name { get; set; } = "";
        public string? AddressJson { get; set; }
        public string Timezone { get; set; } = "UTC";
    }

    public class SiteUpdateDto
    {
        public string? Name { get; set; }
        public string? AddressJson { get; set; }
        public string? Timezone { get; set; }
    }

    public class SiteSearchQuery
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}