using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int SiteId { get; set; }

    public string RoomName { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public string? AttributesJson { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public virtual Site Site { get; set; } = null!;
}
