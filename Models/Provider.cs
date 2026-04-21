using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Provider
{
    public int ProviderId { get; set; }

    public string Name { get; set; } = null!;

    public string? Specialty { get; set; }

    public string? Credentials { get; set; }

    public string? ContactInfo { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? User { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AvailabilityBlock> AvailabilityBlocks { get; set; } = new List<AvailabilityBlock>();

    public virtual ICollection<AvailabilityTemplate> AvailabilityTemplates { get; set; } = new List<AvailabilityTemplate>();

    public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public virtual ICollection<ChargeRef> ChargeRefs { get; set; } = new List<ChargeRef>();

    public virtual ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();

    public virtual ICollection<PublishedSlot> PublishedSlots { get; set; } = new List<PublishedSlot>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
