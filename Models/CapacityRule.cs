using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class CapacityRule
{
    public int RuleId { get; set; }

    public string Scope { get; set; } = null!;

    public int? ScopeId { get; set; }

    public int? MaxApptsPerDay { get; set; }

    public int? MaxConcurrentRooms { get; set; }

    public int BufferMin { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string Status { get; set; } = null!;
}
