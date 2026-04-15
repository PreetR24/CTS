using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareSchedule.Models;

public partial class ResourceHold
{
    public int HoldId { get; set; }

    public string ResourceType { get; set; } = null!;

    public int ResourceId { get; set; }

[Required]
    public int SiteId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public virtual Site Site { get; set; } = null!;
}
