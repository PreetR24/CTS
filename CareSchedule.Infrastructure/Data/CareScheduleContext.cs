using System;
using CareSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Infrastructure.Data;

public partial class CareScheduleContext : DbContext
{
    public CareScheduleContext()
    {
    }

    public CareScheduleContext(DbContextOptions<CareScheduleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentChange> AppointmentChanges { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<AvailabilityBlock> AvailabilityBlocks { get; set; }

    public virtual DbSet<AvailabilityTemplate> AvailabilityTemplates { get; set; }

    public virtual DbSet<Blackout> Blackouts { get; set; }

    public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }

    public virtual DbSet<CapacityRule> CapacityRules { get; set; }

    public virtual DbSet<ChargeRef> ChargeRefs { get; set; }

    public virtual DbSet<CheckIn> CheckIns { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<LeaveImpact> LeaveImpacts { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OnCallCoverage> OnCallCoverages { get; set; }

    public virtual DbSet<OpsReport> OpsReports { get; set; }

    public virtual DbSet<Outcome> Outcomes { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<ProviderService> ProviderServices { get; set; }

    public virtual DbSet<PublishedSlot> PublishedSlots { get; set; }

    public virtual DbSet<ReminderSchedule> ReminderSchedules { get; set; }

    public virtual DbSet<ResourceHold> ResourceHolds { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Roster> Rosters { get; set; }

    public virtual DbSet<RosterAssignment> RosterAssignments { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ShiftTemplate> ShiftTemplates { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<Sla> Slas { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // intentionally empty; configuration is handled externally via DI
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2E65367EE");

            entity.ToTable("Appointment");

            entity.HasIndex(e => e.PatientId, "idx_appointment_patient");

            entity.HasIndex(e => new { e.ProviderId, e.SlotDate }, "idx_appointment_provider_date");

            entity.HasIndex(e => new { e.SiteId, e.SlotDate }, "idx_appointment_site_date");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.BookingChannel).HasMaxLength(30);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Booked");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__4D5F7D71");

            entity.HasOne(d => d.Provider).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Provi__4E53A1AA");

            entity.HasOne(d => d.Room).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Appointme__RoomI__51300E55");

            entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Servi__503BEA1C");

            entity.HasOne(d => d.Site).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__SiteI__4F47C5E3");
        });

        modelBuilder.Entity<AppointmentChange>(entity =>
        {
            entity.HasKey(e => e.ChangeId).HasName("PK__Appointm__0E05C5B7E222AEF0");

            entity.ToTable("AppointmentChange");

            entity.Property(e => e.ChangeId).HasColumnName("ChangeID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ChangeType).HasMaxLength(30);
            entity.Property(e => e.ChangedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NewValuesJson).HasColumnName("NewValuesJSON");
            entity.Property(e => e.OldValuesJson).HasColumnName("OldValuesJSON");
            entity.Property(e => e.Reason).HasMaxLength(255);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentChanges)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__Appointme__Appoi__56E8E7AB");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.AppointmentChanges)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Appointme__Chang__58D1301D");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B8C0A06457");

            entity.ToTable("AuditLog");

            entity.HasIndex(e => new { e.UserId, e.Timestamp }, "idx_auditlog_user_time");

            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Resource).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__AuditLog__UserID__14270015");
        });

        modelBuilder.Entity<AvailabilityBlock>(entity =>
        {
            entity.HasKey(e => e.BlockId).HasName("PK__Availabi__1442151191DB61C9");

            entity.ToTable("AvailabilityBlock");

            entity.Property(e => e.BlockId).HasColumnName("BlockID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Provider).WithMany(p => p.AvailabilityBlocks)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK__Availabil__Provi__3C34F16F");

            entity.HasOne(d => d.Site).WithMany(p => p.AvailabilityBlocks)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Availabil__SiteI__3D2915A8");
        });

        modelBuilder.Entity<AvailabilityTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Availabi__F87ADD0781DF26FF");

            entity.ToTable("AvailabilityTemplate");

            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.SlotDurationMin).HasDefaultValue(15);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Provider).WithMany(p => p.AvailabilityTemplates)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK__Availabil__Provi__3493CFA7");

            entity.HasOne(d => d.Site).WithMany(p => p.AvailabilityTemplates)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Availabil__SiteI__3587F3E0");
        });

        modelBuilder.Entity<Blackout>(entity =>
        {
            entity.HasKey(e => e.BlackoutId).HasName("PK__Blackout__B868459A22B30DDD");

            entity.ToTable("Blackout");

            entity.Property(e => e.BlackoutId).HasColumnName("BlackoutID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Site).WithMany(p => p.Blackouts)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK__Blackout__SiteID__41EDCAC5");
        });

        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Calendar__7944C87055CCF4C3");

            entity.ToTable("CalendarEvent");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType).HasMaxLength(20);
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Provider).WithMany(p => p.CalendarEvents)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__CalendarE__Provi__7B264821");

            entity.HasOne(d => d.Room).WithMany(p => p.CalendarEvents)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__CalendarE__RoomI__7D0E9093");

            entity.HasOne(d => d.Site).WithMany(p => p.CalendarEvents)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CalendarE__SiteI__7C1A6C5A");
        });

        modelBuilder.Entity<CapacityRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__Capacity__110458C257D8B4CC");

            entity.ToTable("CapacityRule");

            entity.Property(e => e.RuleId).HasColumnName("RuleID");
            entity.Property(e => e.Scope).HasMaxLength(20);
            entity.Property(e => e.ScopeId).HasColumnName("ScopeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<ChargeRef>(entity =>
        {
            entity.HasKey(e => e.ChargeRefId).HasName("PK__ChargeRe__635BEE2674BCA41A");

            entity.ToTable("ChargeRef");

            entity.Property(e => e.ChargeRefId).HasColumnName("ChargeRefID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("INR")
                .IsFixedLength();
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Appointment).WithMany(p => p.ChargeRefs)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChargeRef__Appoi__373B3228");

            entity.HasOne(d => d.Provider).WithMany(p => p.ChargeRefs)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChargeRef__Provi__39237A9A");

            entity.HasOne(d => d.Service).WithMany(p => p.ChargeRefs)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChargeRef__Servi__382F5661");
        });

        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.HasKey(e => e.CheckInId).HasName("PK__CheckIn__E64976A4A6077818");

            entity.ToTable("CheckIn");

            entity.HasIndex(e => e.AppointmentId, "UQ__CheckIn__8ECDFCA3CEF40C5B").IsUnique();

            entity.Property(e => e.CheckInId).HasColumnName("CheckInID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.CheckInTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Waiting");
            entity.Property(e => e.TokenNo).HasMaxLength(20);

            entity.HasOne(d => d.Appointment).WithOne(p => p.CheckIn)
                .HasForeignKey<CheckIn>(d => d.AppointmentId)
                .HasConstraintName("FK__CheckIn__Appoint__671F4F74");

            entity.HasOne(d => d.RoomAssignedNavigation).WithMany(p => p.CheckIns)
                .HasForeignKey(d => d.RoomAssigned)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__CheckIn__RoomAss__690797E6");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__Holiday__2D35D59AE05337BA");

            entity.ToTable("Holiday");

            entity.Property(e => e.HolidayId).HasColumnName("HolidayID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Site).WithMany(p => p.Holidays)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK__Holiday__SiteID__4589517F");
        });

        modelBuilder.Entity<LeaveImpact>(entity =>
        {
            entity.HasKey(e => e.ImpactId).HasName("PK__LeaveImp__2297C5DD3AFBF3F9");

            entity.ToTable("LeaveImpact");

            entity.Property(e => e.ImpactId).HasColumnName("ImpactID");
            entity.Property(e => e.ImpactJson).HasColumnName("ImpactJSON");
            entity.Property(e => e.ImpactType).HasMaxLength(30);
            entity.Property(e => e.LeaveId).HasColumnName("LeaveID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Leave).WithMany(p => p.LeaveImpacts)
                .HasForeignKey(d => d.LeaveId)
                .HasConstraintName("FK__LeaveImpa__Leave__214BF109");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.LeaveImpacts)
                .HasForeignKey(d => d.ResolvedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__LeaveImpa__Resol__2334397B");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__LeaveReq__796DB9793791323E");

            entity.ToTable("LeaveRequest");

            entity.HasIndex(e => new { e.UserId, e.Status }, "idx_leaverequest_user");

            entity.Property(e => e.LeaveId).HasColumnName("LeaveID");
            entity.Property(e => e.LeaveType).HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubmittedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__UserI__1A9EF37A");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E320934C661");

            entity.ToTable("Notification");

            entity.HasIndex(e => new { e.UserId, e.Status }, "idx_notification_user");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.Category).HasMaxLength(30);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Unread");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__29E1370A");
        });

        modelBuilder.Entity<OnCallCoverage>(entity =>
        {
            entity.HasKey(e => e.OnCallId).HasName("PK__OnCallCo__B7A67633E4910699");

            entity.ToTable("OnCallCoverage");

            entity.Property(e => e.OnCallId).HasColumnName("OnCallID");
            entity.Property(e => e.BackupUserId).HasColumnName("BackupUserID");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.PrimaryUserId).HasColumnName("PrimaryUserID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.BackupUser).WithMany(p => p.OnCallCoverageBackupUsers)
                .HasForeignKey(d => d.BackupUserId)
                .HasConstraintName("FK__OnCallCov__Backu__15DA3E5D");

            entity.HasOne(d => d.PrimaryUser).WithMany(p => p.OnCallCoveragePrimaryUsers)
                .HasForeignKey(d => d.PrimaryUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OnCallCov__Prima__14E61A24");

            entity.HasOne(d => d.Site).WithMany(p => p.OnCallCoverages)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OnCallCov__SiteI__13F1F5EB");
        });

        modelBuilder.Entity<OpsReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__OpsRepor__D5BD48E5832B3C85");

            entity.ToTable("OpsReport");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.GeneratedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MetricsJson).HasColumnName("MetricsJSON");
            entity.Property(e => e.Scope).HasMaxLength(100);
        });

        modelBuilder.Entity<Outcome>(entity =>
        {
            entity.HasKey(e => e.OutcomeId).HasName("PK__Outcome__113E6AFCC95A983E");

            entity.ToTable("Outcome");

            entity.HasIndex(e => e.AppointmentId, "UQ__Outcome__8ECDFCA383D3EA6B").IsUnique();

            entity.Property(e => e.OutcomeId).HasColumnName("OutcomeID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.MarkedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Outcome1)
                .HasMaxLength(20)
                .HasColumnName("Outcome");

            entity.HasOne(d => d.Appointment).WithOne(p => p.Outcome)
                .HasForeignKey<Outcome>(d => d.AppointmentId)
                .HasConstraintName("FK__Outcome__Appoint__6EC0713C");

            entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.Outcomes)
                .HasForeignKey(d => d.MarkedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Outcome__MarkedB__70A8B9AE");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK__Provider__B54C689D9E0B5E1B");

            entity.ToTable("Provider");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ContactInfo).HasMaxLength(200);
            entity.Property(e => e.Credentials).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Specialty).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<ProviderService>(entity =>
        {
            entity.HasKey(e => e.Psid).HasName("PK__Provider__BC00097697A34DEC");

            entity.ToTable("ProviderService");

            entity.HasIndex(e => new { e.ProviderId, e.ServiceId }, "UQ__Provider__091DD39287307AAF").IsUnique();

            entity.Property(e => e.Psid).HasColumnName("PSID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Provider).WithMany(p => p.ProviderServices)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK__ProviderS__Provi__2EDAF651");

            entity.HasOne(d => d.Service).WithMany(p => p.ProviderServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderS__Servi__2FCF1A8A");
        });

        modelBuilder.Entity<PublishedSlot>(entity =>
        {
            entity.HasKey(e => e.PubSlotId).HasName("PK__Publishe__3CFE843623B80371");

            entity.ToTable("PublishedSlot");

            entity.HasIndex(e => new { e.ProviderId, e.SlotDate }, "idx_publishedslot_provider_date");

            entity.Property(e => e.PubSlotId).HasColumnName("PubSlotID");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Provider).WithMany(p => p.PublishedSlots)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK__Published__Provi__46B27FE2");

            entity.HasOne(d => d.Service).WithMany(p => p.PublishedSlots)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Published__Servi__489AC854");

            entity.HasOne(d => d.Site).WithMany(p => p.PublishedSlots)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Published__SiteI__47A6A41B");
        });

        modelBuilder.Entity<ReminderSchedule>(entity =>
        {
            entity.HasKey(e => e.RemindId).HasName("PK__Reminder__C0874AB5911F58AF");

            entity.ToTable("ReminderSchedule");

            entity.Property(e => e.RemindId).HasColumnName("RemindID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Appointment).WithMany(p => p.ReminderSchedules)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__ReminderS__Appoi__308E3499");
        });

        modelBuilder.Entity<ResourceHold>(entity =>
        {
            entity.HasKey(e => e.HoldId).HasName("PK__Resource__6E24DA24CC77EF60");

            entity.ToTable("ResourceHold");

            entity.Property(e => e.HoldId).HasColumnName("HoldID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");
            entity.Property(e => e.ResourceType).HasMaxLength(20);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Held");

            entity.HasOne(d => d.Site).WithMany(p => p.ResourceHolds)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ResourceH__SiteI__756D6ECB");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__328639196693EC23");

            entity.ToTable("Room");

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.AttributesJson).HasColumnName("AttributesJSON");
            entity.Property(e => e.RoomName).HasMaxLength(100);
            entity.Property(e => e.RoomType).HasMaxLength(30);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Site).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK__Room__SiteID__1CBC4616");
        });

        modelBuilder.Entity<Roster>(entity =>
        {
            entity.HasKey(e => e.RosterId).HasName("PK__Roster__66F6BAAA409A6564");

            entity.ToTable("Roster");

            entity.Property(e => e.RosterId).HasColumnName("RosterID");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.PublishedByNavigation).WithMany(p => p.Rosters)
                .HasForeignKey(d => d.PublishedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Roster__Publishe__0880433F");

            entity.HasOne(d => d.Site).WithMany(p => p.Rosters)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Roster__SiteID__078C1F06");
        });

        modelBuilder.Entity<RosterAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__RosterAs__32499E57E16BA1A5");

            entity.ToTable("RosterAssignment");

            entity.HasIndex(e => new { e.UserId, e.Date }, "idx_rosterassignment_user_date");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.RosterId).HasColumnName("RosterID");
            entity.Property(e => e.ShiftTemplateId).HasColumnName("ShiftTemplateID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Assigned");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Roster).WithMany(p => p.RosterAssignments)
                .HasForeignKey(d => d.RosterId)
                .HasConstraintName("FK__RosterAss__Roste__0D44F85C");

            entity.HasOne(d => d.ShiftTemplate).WithMany(p => p.RosterAssignments)
                .HasForeignKey(d => d.ShiftTemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RosterAss__Shift__0F2D40CE");

            entity.HasOne(d => d.User).WithMany(p => p.RosterAssignments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RosterAss__UserI__0E391C95");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EA83FCDB6E");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.DefaultDurationMin).HasDefaultValue(30);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.VisitType).HasMaxLength(30);
        });

        modelBuilder.Entity<ShiftTemplate>(entity =>
        {
            entity.HasKey(e => e.ShiftTemplateId).HasName("PK__ShiftTem__FA9F704FCAB8D6EC");

            entity.ToTable("ShiftTemplate");

            entity.Property(e => e.ShiftTemplateId).HasColumnName("ShiftTemplateID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Site).WithMany(p => p.ShiftTemplates)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShiftTemp__SiteI__02C769E9");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.SiteId).HasName("PK__Site__B9DCB9037BADE0A3");

            entity.ToTable("Site");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.AddressJson).HasColumnName("AddressJSON");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Timezone)
                .HasMaxLength(60)
                .HasDefaultValue("UTC");
        });

        modelBuilder.Entity<Sla>(entity =>
        {
            entity.HasKey(e => e.Slaid).HasName("PK__SLA__2848A229D3AB2322");

            entity.ToTable("SLA");

            entity.Property(e => e.Slaid).HasColumnName("SLAID");
            entity.Property(e => e.Metric).HasMaxLength(30);
            entity.Property(e => e.Scope).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .HasDefaultValue("Minutes");
        });

        modelBuilder.Entity<SystemConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__SystemCo__C3BC333C2E0516F0");

            entity.ToTable("SystemConfig");

            entity.HasIndex(e => e.Key, "UQ__SystemCo__C41E02899276EC84").IsUnique();

            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Scope)
                .HasMaxLength(20)
                .HasDefaultValue("Global");
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SystemConfigs)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__SystemCon__Updat__41B8C09B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACA628FF66");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053460A18078").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Provider).WithMany()
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__User__ProviderID");
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => e.WaitId).HasName("PK__Waitlist__815F96AC4369AB39");

            entity.ToTable("Waitlist");

            entity.Property(e => e.WaitId).HasColumnName("WaitID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("Normal");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Patient).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__Patien__5F7E2DAC");

            entity.HasOne(d => d.Provider).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__Provid__5D95E53A");

            entity.HasOne(d => d.Service).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__Servic__5E8A0973");

            entity.HasOne(d => d.Site).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__SiteID__5CA1C101");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
