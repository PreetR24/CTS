using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Site
{
    public int SiteId { get; set; }

    public string Name { get; set; } = null!;

    public string? AddressJson { get; set; }

    public string Timezone { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AvailabilityBlock> AvailabilityBlocks { get; set; } = new List<AvailabilityBlock>();

    public virtual ICollection<AvailabilityTemplate> AvailabilityTemplates { get; set; } = new List<AvailabilityTemplate>();

    public virtual ICollection<Blackout> Blackouts { get; set; } = new List<Blackout>();

    public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();

    public virtual ICollection<OnCallCoverage> OnCallCoverages { get; set; } = new List<OnCallCoverage>();

    public virtual ICollection<PublishedSlot> PublishedSlots { get; set; } = new List<PublishedSlot>();

    public virtual ICollection<ResourceHold> ResourceHolds { get; set; } = new List<ResourceHold>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Roster> Rosters { get; set; } = new List<Roster>();

    public virtual ICollection<ShiftTemplate> ShiftTemplates { get; set; } = new List<ShiftTemplate>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
