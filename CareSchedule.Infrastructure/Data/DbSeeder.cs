using CareSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(CareScheduleContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var now = DateTime.UtcNow;

        var site = new Site
        {
            Name = "Main Care Center",
            AddressJson = "{\"line1\":\"123 Main St\",\"city\":\"Chennai\"}",
            Timezone = "Asia/Kolkata",
            Status = "Active"
        };
        db.Sites.Add(site);

        var provider = new Provider
        {
            Name = "Dr. Provider",
            Specialty = "General Medicine",
            Credentials = "MBBS",
            ContactInfo = "doctor@care.com",
            Status = "Active"
        };
        db.Providers.Add(provider);

        var service = new Service
        {
            Name = "General Consultation",
            VisitType = "OPD",
            DefaultDurationMin = 30,
            BufferBeforeMin = 5,
            BufferAfterMin = 5,
            Status = "Active"
        };
        db.Services.Add(service);

        await db.SaveChangesAsync();

        var room = new Room
        {
            SiteId = site.SiteId,
            RoomName = "Room A",
            RoomType = "Consultation",
            AttributesJson = "{\"floor\":1}",
            Status = "Active"
        };
        db.Rooms.Add(room);

        var adminUser = new User
        {
            Name = "Admin User",
            Role = "Admin",
            Email = "admin@care.com",
            Phone = "9999999999",
            Status = "Active"
        };
        var providerUser = new User
        {
            Name = "Provider User",
            Role = "Provider",
            Email = "doctor@care.com",
            Phone = "8888888888",
            Status = "Active",
            ProviderId = provider.ProviderId
        };
        var patientUser = new User
        {
            Name = "Patient User",
            Role = "Patient",
            Email = "patient@care.com",
            Phone = "7777777777",
            Status = "Active"
        };
        db.Users.AddRange(adminUser, providerUser, patientUser);
        await db.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patientUser.UserId,
            ProviderId = provider.ProviderId,
            SiteId = site.SiteId,
            ServiceId = service.ServiceId,
            RoomId = room.RoomId,
            SlotDate = today.AddDays(1),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(10, 30),
            BookingChannel = "Web",
            Status = "Booked"
        };
        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();

        var availabilityTemplate = new AvailabilityTemplate
        {
            ProviderId = provider.ProviderId,
            SiteId = site.SiteId,
            DayOfWeek = (byte)today.DayOfWeek,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDurationMin = 30,
            Status = "Active"
        };
        var availabilityBlock = new AvailabilityBlock
        {
            ProviderId = provider.ProviderId,
            SiteId = site.SiteId,
            Date = today.AddDays(2),
            StartTime = new TimeOnly(13, 0),
            EndTime = new TimeOnly(14, 0),
            Reason = "Training",
            Status = "Active"
        };
        var blackout = new Blackout
        {
            SiteId = site.SiteId,
            StartDate = today.AddDays(10),
            EndDate = today.AddDays(10),
            Reason = "Maintenance",
            Status = "Active"
        };
        var holiday = new Holiday
        {
            SiteId = site.SiteId,
            Date = today.AddDays(20),
            Description = "Public Holiday",
            Status = "Active"
        };
        var providerService = new ProviderService
        {
            ProviderId = provider.ProviderId,
            ServiceId = service.ServiceId,
            CustomDurationMin = 25,
            Status = "Active"
        };
        var publishedSlot = new PublishedSlot
        {
            ProviderId = provider.ProviderId,
            SiteId = site.SiteId,
            ServiceId = service.ServiceId,
            SlotDate = today.AddDays(1),
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(11, 30),
            Status = "Open"
        };
        var appointmentChange = new AppointmentChange
        {
            AppointmentId = appointment.AppointmentId,
            ChangeType = "Created",
            OldValuesJson = null,
            NewValuesJson = "{\"status\":\"Booked\"}",
            ChangedBy = adminUser.UserId,
            ChangedDate = now,
            Reason = "Initial booking"
        };
        var reminder = new ReminderSchedule
        {
            AppointmentId = appointment.AppointmentId,
            RemindOffsetMin = 60,
            Channel = "SMS",
            Status = "Pending"
        };
        var checkIn = new CheckIn
        {
            AppointmentId = appointment.AppointmentId,
            TokenNo = "A001",
            CheckInTime = now,
            RoomAssigned = room.RoomId,
            Status = "Waiting"
        };
        var outcome = new Outcome
        {
            AppointmentId = appointment.AppointmentId,
            Outcome1 = "Completed",
            Notes = "Recovered well",
            MarkedBy = providerUser.UserId,
            MarkedDate = now
        };
        var charge = new ChargeRef
        {
            AppointmentId = appointment.AppointmentId,
            ServiceId = service.ServiceId,
            ProviderId = provider.ProviderId,
            Amount = 500.00m,
            Currency = "INR",
            Status = "Open"
        };
        var audit = new AuditLog
        {
            UserId = adminUser.UserId,
            Action = "Seed",
            Resource = "Database",
            Timestamp = now,
            Metadata = "{\"seeded\":true}"
        };
        var notification = new Notification
        {
            UserId = providerUser.UserId,
            Message = "New appointment assigned",
            Category = "Appointment",
            Status = "Unread",
            CreatedDate = now
        };
        var waitlist = new Waitlist
        {
            SiteId = site.SiteId,
            ProviderId = provider.ProviderId,
            ServiceId = service.ServiceId,
            PatientId = patientUser.UserId,
            Priority = "Normal",
            RequestedDate = today,
            Status = "Open"
        };
        var calendarEvent = new CalendarEvent
        {
            EntityType = "Appointment",
            EntityId = appointment.AppointmentId,
            ProviderId = provider.ProviderId,
            SiteId = site.SiteId,
            RoomId = room.RoomId,
            StartTime = now.AddDays(1),
            EndTime = now.AddDays(1).AddMinutes(30),
            Status = "Active"
        };
        var resourceHold = new ResourceHold
        {
            ResourceType = "Room",
            ResourceId = room.RoomId,
            SiteId = site.SiteId,
            StartTime = now.AddDays(1),
            EndTime = now.AddDays(1).AddMinutes(30),
            Reason = "Reserved for appointment",
            Status = "Held"
        };
        var shiftTemplate = new ShiftTemplate
        {
            Name = "Morning Shift",
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            BreakMinutes = 30,
            Role = "Provider",
            SiteId = site.SiteId,
            Status = "Active"
        };
        db.AddRange(
            availabilityTemplate,
            availabilityBlock,
            blackout,
            holiday,
            providerService,
            publishedSlot,
            appointmentChange,
            reminder,
            checkIn,
            outcome,
            charge,
            audit,
            notification,
            waitlist,
            calendarEvent,
            resourceHold,
            shiftTemplate
        );
        await db.SaveChangesAsync();

        var roster = new Roster
        {
            SiteId = site.SiteId,
            Department = "General",
            PeriodStart = today,
            PeriodEnd = today.AddDays(6),
            PublishedBy = adminUser.UserId,
            PublishedDate = now,
            Status = "Published"
        };
        db.Rosters.Add(roster);

        var leaveRequest = new LeaveRequest
        {
            UserId = providerUser.UserId,
            LeaveType = "Sick",
            StartDate = today.AddDays(5),
            EndDate = today.AddDays(6),
            Reason = "Flu",
            SubmittedDate = now,
            Status = "Pending"
        };
        db.LeaveRequests.Add(leaveRequest);
        await db.SaveChangesAsync();

        var rosterAssignment = new RosterAssignment
        {
            RosterId = roster.RosterId,
            UserId = providerUser.UserId,
            ShiftTemplateId = shiftTemplate.ShiftTemplateId,
            Date = today.AddDays(1),
            Role = "Doctor",
            Status = "Assigned"
        };
        var onCall = new OnCallCoverage
        {
            SiteId = site.SiteId,
            Department = "General",
            Date = today.AddDays(1),
            StartTime = new TimeOnly(20, 0),
            EndTime = new TimeOnly(23, 0),
            PrimaryUserId = providerUser.UserId,
            BackupUserId = adminUser.UserId,
            Status = "Active"
        };
        var leaveImpact = new LeaveImpact
        {
            LeaveId = leaveRequest.LeaveId,
            ImpactType = "Schedule",
            ImpactJson = "{\"affectedAppointments\":1}",
            ResolvedBy = null,
            ResolvedDate = null,
            Status = "Open"
        };
        var capacityRule = new CapacityRule
        {
            Scope = "Site",
            ScopeId = site.SiteId,
            MaxApptsPerDay = 50,
            MaxConcurrentRooms = 10,
            BufferMin = 5,
            EffectiveFrom = today,
            EffectiveTo = null,
            Status = "Active"
        };
        var sla = new Sla
        {
            Scope = "Appointment",
            Metric = "WaitTime",
            TargetValue = 20,
            Unit = "Minutes",
            Status = "Active"
        };
        var systemConfig = new SystemConfig
        {
            Key = "BookingWindowDays",
            Value = "30",
            Scope = "Global",
            UpdatedBy = adminUser.UserId,
            UpdatedDate = now
        };
        var opsReport = new OpsReport
        {
            Scope = "Daily",
            MetricsJson = "{\"bookings\":1}",
            GeneratedDate = now
        };
        db.AddRange(rosterAssignment, onCall, leaveImpact, capacityRule, sla, systemConfig, opsReport);
        await db.SaveChangesAsync();
    }
}
