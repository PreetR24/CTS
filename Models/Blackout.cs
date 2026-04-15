using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Blackout
{
    public int BlackoutId { get; set; }

    public int SiteId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
