using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class AvailabilityTemplate
{
    public int TemplateId { get; set; }

    public int ProviderId { get; set; }

    public int SiteId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int SlotDurationMin { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
