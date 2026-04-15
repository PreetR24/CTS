using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class ShiftTemplate
{
    public int ShiftTemplateId { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int BreakMinutes { get; set; }

    public string Role { get; set; } = null!;

    public int SiteId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<RosterAssignment> RosterAssignments { get; set; } = new List<RosterAssignment>();

    public virtual Site Site { get; set; } = null!;
}
