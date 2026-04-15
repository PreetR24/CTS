using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class PublishedSlot
{
    public int PubSlotId { get; set; }

    public int ProviderId { get; set; }

    public int SiteId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
