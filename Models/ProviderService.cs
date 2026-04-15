using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class ProviderService
{
    public int Psid { get; set; }

    public int ProviderId { get; set; }

    public int ServiceId { get; set; }

    public int? CustomDurationMin { get; set; }

    public int? CustomBufferBeforeMin { get; set; }

    public int? CustomBufferAfterMin { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
