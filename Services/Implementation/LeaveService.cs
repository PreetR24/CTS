using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class LeaveService(
            ILeaveRequestRepository _leaveRepo,
            ILeaveImpactRepository _impactRepo,
            IAppointmentRepository _apptRepo,
            IRosterAssignmentRepository _rosterAssignRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            IAvailabilityBlockRepository _blockRepo,
            ICalendarEventRepository _calendarRepo,
            IProviderRepository _providerRepo,
            IUnitOfWork _uow) : ILeaveService
    {
        private static readonly string[] AllowedLeaveTypes = { "Vacation", "Sick", "CME", "Other" };

        public LeaveRequestResponseDto Submit(int userId, CreateLeaveRequestDto dto)
        {
            if (!AllowedLeaveTypes.Contains(dto.LeaveType, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"LeaveType must be one of: {string.Join(", ", AllowedLeaveTypes)}.");

            var startDate = ParseDate(dto.StartDate, "StartDate");
            var endDate = ParseDate(dto.EndDate, "EndDate");
            if (startDate > endDate)
                throw new ArgumentException("StartDate must be on or before EndDate.");

            var overlapping = _leaveRepo.Search(userId, null)
                .Any(l => l.Status is "Pending" or "Approved"
                    && l.StartDate <= endDate && l.EndDate >= startDate);
            if (overlapping)
                throw new ArgumentException("Overlapping leave request exists for this date range.");

            var entity = new LeaveRequest
            {
                UserId = userId,
                LeaveType = dto.LeaveType,
                StartDate = startDate,
                EndDate = endDate,
                Reason = dto.Reason,
                SubmittedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _leaveRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Submitted",
                Resource = "LeaveRequest",
                Metadata = $"{{\"leaveId\":{entity.LeaveId},\"userId\":{userId}}}"
            });

            _uow.SaveChanges();
            return MapLeave(entity);
        }

        public LeaveRequestResponseDto Cancel(int leaveId, int userId)
        {
            var entity = _leaveRepo.GetById(leaveId)
                ?? throw new KeyNotFoundException("Leave request not found.");

            if (entity.UserId != userId)
                throw new ArgumentException("You can only cancel your own leave requests.");

            if (entity.Status != "Pending")
                throw new ArgumentException("Only pending leave requests can be cancelled.");

            entity.Status = "Cancelled";
            _leaveRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Cancelled",
                Resource = "LeaveRequest",
                Metadata = $"{{\"leaveId\":{leaveId}}}"
            });

            _uow.SaveChanges();
            return MapLeave(entity);
        }

        public LeaveRequestResponseDto GetById(int leaveId)
        {
            var entity = _leaveRepo.GetById(leaveId)
                ?? throw new KeyNotFoundException("Leave request not found.");
            return MapLeave(entity);
        }

        public IEnumerable<LeaveRequestResponseDto> Search(LeaveSearchDto dto)
        {
            return _leaveRepo.Search(dto.UserId, dto.Status)
                .Select(MapLeave).ToList();
        }

        public LeaveRequestResponseDto Approve(int leaveId)
        {
            var entity = _leaveRepo.GetById(leaveId)
                ?? throw new KeyNotFoundException("Leave request not found.");

            if (entity.Status != "Pending")
                throw new ArgumentException("Only pending leave requests can be approved.");

            entity.Status = "Approved";
            _leaveRepo.Update(entity);

            var providerId = _providerRepo.GetById(entity.UserId)?.ProviderId;

            // Create AvailabilityBlocks + CalendarEvents for each day in leave range
            if (providerId.HasValue)
            {
                var siteId = 0;
                var providerAppointments = _apptRepo.Search(null, providerId.Value, null, null, null).ToList();
                if (providerAppointments.Count > 0)
                    siteId = providerAppointments.First().SiteId;

                for (var d = entity.StartDate; d <= entity.EndDate; d = d.AddDays(1))
                {
                    var block = new AvailabilityBlock
                    {
                        ProviderId = providerId.Value,
                        SiteId = siteId,
                        Date = d,
                        StartTime = new TimeOnly(0, 0),
                        EndTime = new TimeOnly(23, 59),
                        Reason = $"Leave: {entity.LeaveType}",
                        Status = "Active"
                    };
                    _blockRepo.Add(block);

                    _uow.SaveChanges();

                    _calendarRepo.Add(new CalendarEvent
                    {
                        EntityType = "AvailabilityBlock",
                        EntityId = block.BlockId,
                        ProviderId = providerId.Value,
                        SiteId = siteId,
                        RoomId = null,
                        StartTime = d.ToDateTime(new TimeOnly(0, 0)),
                        EndTime = d.ToDateTime(new TimeOnly(23, 59)),
                        Status = "Leave"
                    });
                }

                // Generate LeaveImpact for affected appointments
                var affectedAppts = providerAppointments
                    .Where(a => a.SlotDate >= entity.StartDate && a.SlotDate <= entity.EndDate
                        && a.Status is not ("Cancelled" or "Completed" or "NoShow"))
                    .ToList();

                if (affectedAppts.Count > 0)
                {
                    var appointmentIds = affectedAppts.Select(a => a.AppointmentId).ToList();
                    _impactRepo.Add(new LeaveImpact
                    {
                        LeaveId = leaveId,
                        ImpactType = "Appointments",
                        ImpactJson = JsonSerializer.Serialize(new { appointmentIds }),
                        Status = "Open"
                    });

                    foreach (var appt in affectedAppts)
                    {
                        _notifRepo.Add(new Notification
                        {
                            UserId = appt.PatientId,
                            Message = $"Your appointment on {appt.SlotDate:yyyy-MM-dd} may need to be rescheduled due to provider leave.",
                            Category = "Appointment",
                            Status = "Unread",
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            // Generate LeaveImpact for affected roster assignments
            var affectedAssignments = _rosterAssignRepo
                .Search(null, entity.UserId, null, "Assigned")
                .Where(a => a.Date >= entity.StartDate && a.Date <= entity.EndDate)
                .ToList();

            if (affectedAssignments.Count > 0)
            {
                var assignmentIds = affectedAssignments.Select(a => a.AssignmentId).ToList();
                _impactRepo.Add(new LeaveImpact
                {
                    LeaveId = leaveId,
                    ImpactType = "Roster",
                    ImpactJson = JsonSerializer.Serialize(new { assignmentIds }),
                    Status = "Open"
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Approved",
                Resource = "LeaveRequest",
                Metadata = $"{{\"leaveId\":{leaveId}}}"
            });

            _uow.SaveChanges();
            return MapLeave(entity);
        }

        public LeaveRequestResponseDto Reject(int leaveId)
        {
            var entity = _leaveRepo.GetById(leaveId)
                ?? throw new KeyNotFoundException("Leave request not found.");

            if (entity.Status != "Pending")
                throw new ArgumentException("Only pending leave requests can be rejected.");

            entity.Status = "Rejected";
            _leaveRepo.Update(entity);

            _notifRepo.Add(new Notification
            {
                UserId = entity.UserId,
                Message = $"Your leave request ({entity.LeaveType}) from {entity.StartDate:yyyy-MM-dd} to {entity.EndDate:yyyy-MM-dd} has been rejected.",
                Category = "Leave",
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            });

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Rejected",
                Resource = "LeaveRequest",
                Metadata = $"{{\"leaveId\":{leaveId}}}"
            });

            _uow.SaveChanges();
            return MapLeave(entity);
        }

        public IEnumerable<LeaveImpactResponseDto> GetImpactsByLeaveId(int leaveId)
        {
            return _impactRepo.GetByLeaveId(leaveId)
                .Select(MapImpact).ToList();
        }

        public LeaveImpactResponseDto GetImpactById(int impactId)
        {
            var entity = _impactRepo.GetById(impactId)
                ?? throw new KeyNotFoundException("Leave impact not found.");
            return MapImpact(entity);
        }

        public LeaveImpactResponseDto CreateImpact(CreateLeaveImpactDto dto)
        {
            var leave = _leaveRepo.GetById(dto.LeaveId)
                ?? throw new KeyNotFoundException("Leave request not found.");

            var entity = new LeaveImpact
            {
                LeaveId = dto.LeaveId,
                ImpactType = dto.ImpactType,
                ImpactJson = dto.ImpactJson,
                Status = "Open"
            };

            _impactRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Created",
                Resource = "LeaveImpact",
                Metadata = $"{{\"impactId\":{entity.ImpactId},\"leaveId\":{dto.LeaveId}}}"
            });

            _uow.SaveChanges();
            return MapImpact(entity);
        }

        public LeaveImpactResponseDto ResolveImpact(int impactId, ResolveLeaveImpactDto dto)
        {
            var entity = _impactRepo.GetById(impactId)
                ?? throw new KeyNotFoundException("Leave impact not found.");

            if (entity.Status != "Open")
                throw new ArgumentException("Only open impacts can be resolved.");

            entity.Status = "Resolved";
            entity.ResolvedBy = dto.ResolvedBy;
            entity.ResolvedDate = DateTime.UtcNow;
            _impactRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Resolved",
                Resource = "LeaveImpact",
                Metadata = $"{{\"impactId\":{impactId}}}"
            });

            _uow.SaveChanges();
            return MapImpact(entity);
        }

        // ===================== Helpers =====================

        private static DateOnly ParseDate(string value, string fieldName)
        {
            if (!DateOnly.TryParseExact(value?.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                throw new ArgumentException($"Invalid {fieldName} format. Use yyyy-MM-dd.");
            return result;
        }

        private static LeaveRequestResponseDto MapLeave(LeaveRequest e) => new()
        {
            LeaveId = e.LeaveId,
            UserId = e.UserId,
            LeaveType = e.LeaveType,
            StartDate = e.StartDate.ToString("yyyy-MM-dd"),
            EndDate = e.EndDate.ToString("yyyy-MM-dd"),
            Reason = e.Reason,
            SubmittedDate = e.SubmittedDate,
            Status = e.Status
        };

        private static LeaveImpactResponseDto MapImpact(LeaveImpact e) => new()
        {
            ImpactId = e.ImpactId,
            LeaveId = e.LeaveId,
            ImpactType = e.ImpactType,
            ImpactJson = e.ImpactJson,
            ResolvedBy = e.ResolvedBy,
            ResolvedDate = e.ResolvedDate,
            Status = e.Status
        };
    }
}
