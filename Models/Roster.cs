using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Roster
{
    public int RosterId { get; set; }

    public int SiteId { get; set; }

    public string? Department { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public int? PublishedBy { get; set; }

    public DateTime? PublishedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? PublishedByNavigation { get; set; }

    public virtual ICollection<RosterAssignment> RosterAssignments { get; set; } = new List<RosterAssignment>();

    public virtual Site Site { get; set; } = null!;
}
