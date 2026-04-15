using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class OpsReport
{
    public int ReportId { get; set; }

    public string Scope { get; set; } = null!;

    public string? MetricsJson { get; set; }

    public DateTime GeneratedDate { get; set; }
}
