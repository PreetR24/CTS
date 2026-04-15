using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Holiday
{
    public int HolidayId { get; set; }

    public int SiteId { get; set; }

    public DateOnly Date { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
