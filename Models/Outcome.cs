using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Outcome
{
    public int OutcomeId { get; set; }

    public int AppointmentId { get; set; }

    public string Outcome1 { get; set; } = null!;

    public string? Notes { get; set; }

    public int? MarkedBy { get; set; }

    public DateTime MarkedDate { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User? MarkedByNavigation { get; set; }
}
