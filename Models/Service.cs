using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string VisitType { get; set; } = null!;

    public int DefaultDurationMin { get; set; }

    public int BufferBeforeMin { get; set; }

    public int BufferAfterMin { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ChargeRef> ChargeRefs { get; set; } = new List<ChargeRef>();

    public virtual ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();

    public virtual ICollection<PublishedSlot> PublishedSlots { get; set; } = new List<PublishedSlot>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
