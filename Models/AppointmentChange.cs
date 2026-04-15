using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class AppointmentChange
{
    public int ChangeId { get; set; }

    public int AppointmentId { get; set; }

    public string ChangeType { get; set; } = null!;

    public string? OldValuesJson { get; set; }

    public string? NewValuesJson { get; set; }

    public int? ChangedBy { get; set; }

    public DateTime ChangedDate { get; set; }

    public string? Reason { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User? ChangedByNavigation { get; set; }
}
