using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class CheckIn
{
    public int CheckInId { get; set; }

    public int AppointmentId { get; set; }

    public string? TokenNo { get; set; }

    public DateTime CheckInTime { get; set; }

    public int? RoomAssigned { get; set; }

    public string Status { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Room? RoomAssignedNavigation { get; set; }
}
