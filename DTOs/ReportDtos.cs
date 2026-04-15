using System;

namespace CareSchedule.DTOs
{
    public class OpsReportResponseDto
    {
        public int ReportId { get; set; }
        public string Scope { get; set; } = "";
        public string? MetricsJson { get; set; }
        public DateTime GeneratedDate { get; set; }
    }

    public class ReportSearchDto
    {
        public string? Scope { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }

    public class CreateReportDto
    {
        public string Scope { get; set; } = "";
        public string? MetricsJson { get; set; }
    }
}
