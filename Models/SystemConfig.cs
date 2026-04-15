using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class SystemConfig
{
    public int ConfigId { get; set; }

    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Scope { get; set; } = null!;

    public int? UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
