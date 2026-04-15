using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class LeaveRequest
{
    public int LeaveId { get; set; }

    public int UserId { get; set; }

    public string LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public DateTime SubmittedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<LeaveImpact> LeaveImpacts { get; set; } = new List<LeaveImpact>();

    public virtual User User { get; set; } = null!;
}
