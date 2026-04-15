using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class ChargeRef
{
    public int ChargeRefId { get; set; }

    public int AppointmentId { get; set; }

    public int ServiceId { get; set; }

    public int ProviderId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
