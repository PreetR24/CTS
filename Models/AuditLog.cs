using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public int? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string Resource { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string? Metadata { get; set; }

    public virtual User? User { get; set; }
}
