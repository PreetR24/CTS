using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class LeaveImpact
{
    public int ImpactId { get; set; }

    public int LeaveId { get; set; }

    public string ImpactType { get; set; } = null!;

    public string? ImpactJson { get; set; }

    public int? ResolvedBy { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual LeaveRequest Leave { get; set; } = null!;

    public virtual User? ResolvedByNavigation { get; set; }
}
