using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class AvailabilityBlock
{
    public int BlockId { get; set; }

    public int ProviderId { get; set; }

    public int SiteId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
