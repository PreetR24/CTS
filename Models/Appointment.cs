using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int ProviderId { get; set; }

    public int SiteId { get; set; }

    public int ServiceId { get; set; }

    public int? RoomId { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string BookingChannel { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<AppointmentChange> AppointmentChanges { get; set; } = new List<AppointmentChange>();

    public virtual ICollection<ChargeRef> ChargeRefs { get; set; } = new List<ChargeRef>();

    public virtual CheckIn? CheckIn { get; set; }

    public virtual Outcome? Outcome { get; set; }

    public virtual User Patient { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual ICollection<ReminderSchedule> ReminderSchedules { get; set; } = new List<ReminderSchedule>();

    public virtual Room? Room { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
