using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class RosterAssignment
{
    public int AssignmentId { get; set; }

    public int RosterId { get; set; }

    public int UserId { get; set; }

    public int ShiftTemplateId { get; set; }

    public DateOnly Date { get; set; }

    public string? Role { get; set; }

    public string Status { get; set; } = null!;

    public virtual Roster Roster { get; set; } = null!;

    public virtual ShiftTemplate ShiftTemplate { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
