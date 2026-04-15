using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class CalendarEvent
{
    public int EventId { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public int? ProviderId { get; set; }

    public int SiteId { get; set; }

    public int? RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider? Provider { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Site Site { get; set; } = null!;
}
