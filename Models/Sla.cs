using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class Sla
{
    public int Slaid { get; set; }

    public string Scope { get; set; } = null!;

    public string Metric { get; set; } = null!;

    public int TargetValue { get; set; }

    public string Unit { get; set; } = null!;

    public string Status { get; set; } = null!;
}
