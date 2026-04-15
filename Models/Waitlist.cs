using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Waitlist
{
    public int WaitId { get; set; }

    public int SiteId { get; set; }

    public int ProviderId { get; set; }

    public int ServiceId { get; set; }

    public int PatientId { get; set; }

    public string Priority { get; set; } = null!;

    public DateOnly RequestedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Patient { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
