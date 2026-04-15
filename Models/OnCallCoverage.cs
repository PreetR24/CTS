using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class OnCallCoverage
{
    public int OnCallId { get; set; }

    public int SiteId { get; set; }

    public string? Department { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int PrimaryUserId { get; set; }

    public int? BackupUserId { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? BackupUser { get; set; }

    public virtual User PrimaryUser { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
