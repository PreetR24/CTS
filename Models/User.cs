using System;
using System.Collections.Generic;

namespace CareSchedule.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Status { get; set; } = null!;

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<AppointmentChange> AppointmentChanges { get; set; } = new List<AppointmentChange>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<LeaveImpact> LeaveImpacts { get; set; } = new List<LeaveImpact>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OnCallCoverage> OnCallCoverageBackupUsers { get; set; } = new List<OnCallCoverage>();

    public virtual ICollection<OnCallCoverage> OnCallCoveragePrimaryUsers { get; set; } = new List<OnCallCoverage>();

    public virtual ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();

    public virtual ICollection<RosterAssignment> RosterAssignments { get; set; } = new List<RosterAssignment>();

    public virtual ICollection<Roster> Rosters { get; set; } = new List<Roster>();

    public virtual ICollection<SystemConfig> SystemConfigs { get; set; } = new List<SystemConfig>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
