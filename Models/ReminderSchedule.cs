using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class ReminderSchedule
{
    public int RemindId { get; set; }

    public int AppointmentId { get; set; }

    public int RemindOffsetMin { get; set; }

    public string Channel { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;
}
