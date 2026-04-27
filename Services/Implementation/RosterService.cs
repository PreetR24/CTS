using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class RosterService(
            IShiftTemplateRepository _shiftRepo,
            IRosterRepository _rosterRepo,
            IRosterAssignmentRepository _assignRepo,
            IOnCallCoverageRepository _onCallRepo,
            IUserRepository _userRepo,
            ISiteRepository _siteRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            CareScheduleContext _db) : IRosterService
    {
        private static readonly string[] AllowedRoles = { "Nurse", "FrontDesk", "Provider" };

        // ===================== Shift Templates =====================

        public ShiftTemplateResponseDto CreateShiftTemplate(CreateShiftTemplateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");
            EnsureSiteActive(dto.SiteId);

            dto.Name = dto.Name.Trim();
            var start = ParseTime(dto.StartTime, "StartTime");
            var end = ParseTime(dto.EndTime, "EndTime");
            if (start >= end)
                throw new ArgumentException("StartTime must be before EndTime.");

            if (!AllowedRoles.Contains(dto.Role, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"Role must be one of: {string.Join(", ", AllowedRoles)}.");

            var hasDuplicate = _shiftRepo.Search(dto.SiteId, dto.Role, null)
                .Any(t =>
                    string.Equals(t.Name?.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    t.StartTime == start &&
                    t.EndTime == end &&
                    !string.Equals(t.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (hasDuplicate)
                throw new ArgumentException("Duplicate shift template exists for same site, role, name and time.");

            var entity = new ShiftTemplate
            {
                Name = dto.Name,
                StartTime = start,
                EndTime = end,
                BreakMinutes = dto.BreakMinutes,
                Role = dto.Role,
                SiteId = dto.SiteId,
                Status = "Active"
            };

            _shiftRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Created",
                Resource = "ShiftTemplate",
                Metadata = $"{{\"name\":\"{entity.Name}\",\"siteId\":{entity.SiteId}}}"
            });

            _db.SaveChanges();
            return MapShiftTemplate(entity);
        }

        public ShiftTemplateResponseDto UpdateShiftTemplate(int id, UpdateShiftTemplateDto dto)
        {
            var entity = _shiftRepo.GetById(id)
                ?? throw new KeyNotFoundException("Shift template not found.");

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.StartTime != null) entity.StartTime = ParseTime(dto.StartTime, "StartTime");
            if (dto.EndTime != null) entity.EndTime = ParseTime(dto.EndTime, "EndTime");
            if (dto.BreakMinutes.HasValue) entity.BreakMinutes = dto.BreakMinutes.Value;
            if (dto.Role != null)
            {
                if (!AllowedRoles.Contains(dto.Role, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Role must be one of: {string.Join(", ", AllowedRoles)}.");
                entity.Role = dto.Role;
            }
            if (dto.Status != null) entity.Status = dto.Status;

            if (entity.StartTime >= entity.EndTime)
                throw new ArgumentException("StartTime must be before EndTime.");

            var hasDuplicate = _shiftRepo.Search(entity.SiteId, entity.Role, null)
                .Any(t =>
                    t.ShiftTemplateId != id &&
                    string.Equals(t.Name?.Trim(), entity.Name?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    t.StartTime == entity.StartTime &&
                    t.EndTime == entity.EndTime &&
                    !string.Equals(t.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (hasDuplicate)
                throw new ArgumentException("Duplicate shift template exists for same site, role, name and time.");

            _shiftRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Updated",
                Resource = "ShiftTemplate",
                Metadata = $"{{\"shiftTemplateId\":{id}}}"
            });

            _db.SaveChanges();
            return MapShiftTemplate(entity);
        }

        public void DeleteShiftTemplate(int id)
        {
            var entity = _shiftRepo.GetById(id)
                ?? throw new KeyNotFoundException("Shift template not found.");

            var activeAssignments = _assignRepo.Search(null, null, null, "Assigned")
                .Any(a => a.ShiftTemplateId == id);
            if (activeAssignments)
                throw new ArgumentException("Cannot delete: active roster assignments reference this template.");

            entity.Status = "Inactive";
            _shiftRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Deleted",
                Resource = "ShiftTemplate",
                Metadata = $"{{\"shiftTemplateId\":{id}}}"
            });

            _db.SaveChanges();
        }

        public ShiftTemplateResponseDto GetShiftTemplate(int id)
        {
            var entity = _shiftRepo.GetById(id)
                ?? throw new KeyNotFoundException("Shift template not found.");
            return MapShiftTemplate(entity);
        }

        public IEnumerable<ShiftTemplateResponseDto> SearchShiftTemplates(ShiftTemplateSearchDto dto)
        {
            return _shiftRepo.Search(dto.SiteId, dto.Role, dto.Status)
                .Select(MapShiftTemplate).ToList();
        }

        // ===================== Rosters =====================

        public RosterResponseDto CreateRoster(CreateRosterDto dto)
        {
            var periodStart = ParseDate(dto.PeriodStart, "PeriodStart");
            var periodEnd = ParseDate(dto.PeriodEnd, "PeriodEnd");
            if (periodStart >= periodEnd)
                throw new ArgumentException("PeriodStart must be before PeriodEnd.");
            EnsureSiteActive(dto.SiteId);

            var entity = new Roster
            {
                SiteId = dto.SiteId,
                Department = dto.Department,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                Status = "Draft"
            };

            _rosterRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Created",
                Resource = "Roster",
                Metadata = $"{{\"siteId\":{dto.SiteId},\"period\":\"{periodStart:yyyy-MM-dd} to {periodEnd:yyyy-MM-dd}\"}}"
            });

            _db.SaveChanges();
            return MapRoster(entity);
        }

        public RosterResponseDto UpdateRoster(int rosterId, UpdateRosterDto dto)
        {
            var entity = _rosterRepo.GetById(rosterId)
                ?? throw new KeyNotFoundException("Roster not found.");

            if (!string.IsNullOrWhiteSpace(dto.Department))
                entity.Department = dto.Department;
            if (!string.IsNullOrWhiteSpace(dto.PeriodStart))
                entity.PeriodStart = ParseDate(dto.PeriodStart, "PeriodStart");
            if (!string.IsNullOrWhiteSpace(dto.PeriodEnd))
                entity.PeriodEnd = ParseDate(dto.PeriodEnd, "PeriodEnd");
            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status;

            if (entity.PeriodStart >= entity.PeriodEnd)
                throw new ArgumentException("PeriodStart must be before PeriodEnd.");

            _rosterRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Updated",
                Resource = "Roster",
                Metadata = $"{{\"rosterId\":{rosterId}}}"
            });

            _db.SaveChanges();
            return MapRoster(entity);
        }

        public void DeleteRoster(int rosterId)
        {
            var entity = _rosterRepo.GetById(rosterId)
                ?? throw new KeyNotFoundException("Roster not found.");

            entity.Status = "Cancelled";
            _rosterRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Deleted",
                Resource = "Roster",
                Metadata = $"{{\"rosterId\":{rosterId}}}"
            });

            _db.SaveChanges();
        }

        public RosterResponseDto PublishRoster(int rosterId, PublishRosterDto dto)
        {
            var entity = _rosterRepo.GetById(rosterId)
                ?? throw new KeyNotFoundException("Roster not found.");

            if (entity.Status != "Draft")
                throw new ArgumentException("Only Draft rosters can be published.");

            entity.Status = "Published";
            entity.PublishedBy = dto.PublishedBy;
            entity.PublishedDate = TimeZoneHelper.NowIst();
            _rosterRepo.Update(entity);

            var assignments = _assignRepo.Search(null, null, null, null)
                .Where(a => a.RosterId == rosterId)
                .ToList();

            var distinctUserIds = assignments.Select(a => a.UserId).Distinct();
            foreach (var userId in distinctUserIds)
            {
                _notifRepo.Add(new Notification
                {
                    UserId = userId,
                    Message = $"Roster for {entity.PeriodStart:MMM dd} - {entity.PeriodEnd:MMM dd} has been published. Check your assignments.",
                    Category = "Roster",
                    Status = "Unread",
                    CreatedDate = TimeZoneHelper.NowIst()
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Published",
                Resource = "Roster",
                Metadata = $"{{\"rosterId\":{rosterId},\"publishedBy\":{dto.PublishedBy}}}"
            });

            _db.SaveChanges();
            return MapRoster(entity);
        }

        public RosterResponseDto GetRoster(int id)
        {
            var entity = _rosterRepo.GetById(id)
                ?? throw new KeyNotFoundException("Roster not found.");
            return MapRoster(entity);
        }

        public IEnumerable<RosterResponseDto> SearchRosters(RosterSearchDto dto)
        {
            return _rosterRepo.Search(dto.SiteId, dto.Status)
                .Select(MapRoster).ToList();
        }

        // ===================== Assignments =====================

        public RosterAssignmentResponseDto AssignStaff(CreateRosterAssignmentDto dto)
        {
            var roster = _rosterRepo.GetById(dto.RosterId)
                ?? throw new KeyNotFoundException("Roster not found.");
            if (roster.Status != "Draft")
                throw new ArgumentException("Can only assign staff to Draft rosters.");

            var shift = _shiftRepo.GetById(dto.ShiftTemplateId)
                ?? throw new KeyNotFoundException("Shift template not found.");
            if (shift.Status != "Active")
                throw new ArgumentException("Shift template is not active.");

            var user = _userRepo.GetById(dto.UserId)
                ?? throw new KeyNotFoundException("User not found.");
            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User is not active.");

            if (!string.Equals(user.Role, shift.Role, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"User role '{user.Role}' does not match shift role '{shift.Role}'.");

            var date = ParseDate(dto.Date, "Date");

            var hasSameTemplateAssignment = _assignRepo.Search(null, dto.UserId, date, null)
                .Any(a =>
                    a.ShiftTemplateId == dto.ShiftTemplateId &&
                    !string.Equals(a.Status, "Absent", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase));
            if (hasSameTemplateAssignment)
                throw new ArgumentException("This nurse already has the same shift template assigned on this date.");

            var entity = new RosterAssignment
            {
                RosterId = dto.RosterId,
                UserId = dto.UserId,
                ShiftTemplateId = dto.ShiftTemplateId,
                Date = date,
                Role = dto.Role ?? shift.Role,
                Status = "Assigned"
            };

            _assignRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Assigned",
                Resource = "RosterAssignment",
                Metadata = $"{{\"rosterId\":{dto.RosterId},\"userId\":{dto.UserId},\"date\":\"{date:yyyy-MM-dd}\"}}"
            });

            _db.SaveChanges();
            return MapAssignment(entity);
        }

        public RosterAssignmentResponseDto SwapShift(int assignmentId, SwapAssignmentDto dto)
        {
            var entity = _assignRepo.GetById(assignmentId)
                ?? throw new KeyNotFoundException("Assignment not found.");
            if (entity.Status != "Assigned")
                throw new ArgumentException("Only Assigned shifts can be swapped.");
            if (!dto.NewUserId.HasValue && !dto.NewShiftTemplateId.HasValue)
                throw new ArgumentException("Provide at least one change (newUserId or newShiftTemplateId).");

            var oldUserId = entity.UserId;

            if (dto.NewUserId.HasValue)
            {
                var newUser = _userRepo.GetById(dto.NewUserId.Value)
                    ?? throw new KeyNotFoundException("New user not found.");
                if (!string.Equals(newUser.Status, "Active", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("New user is not active.");

                var targetShift = dto.NewShiftTemplateId.HasValue
                    ? _shiftRepo.GetById(dto.NewShiftTemplateId.Value) ?? throw new KeyNotFoundException("New shift template not found.")
                    : _shiftRepo.GetById(entity.ShiftTemplateId)!;

                if (!string.Equals(newUser.Role, targetShift.Role, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"New user role '{newUser.Role}' does not match shift role '{targetShift.Role}'.");

                var hasDuplicateAssignment = _assignRepo.Search(null, dto.NewUserId.Value, entity.Date, null)
                    .Any(a =>
                        a.AssignmentId != entity.AssignmentId &&
                        a.ShiftTemplateId == (dto.NewShiftTemplateId ?? entity.ShiftTemplateId) &&
                        !string.Equals(a.Status, "Absent", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase));
                if (hasDuplicateAssignment)
                    throw new ArgumentException("Replacement staff already has this shift template assigned on this date.");

                entity.UserId = dto.NewUserId.Value;
            }

            if (dto.NewShiftTemplateId.HasValue)
                entity.ShiftTemplateId = dto.NewShiftTemplateId.Value;

            entity.Status = "Assigned";
            _assignRepo.Update(entity);

            _notifRepo.Add(new Notification
            {
                UserId = oldUserId,
                Message = $"Your shift on {entity.Date:yyyy-MM-dd} has been swapped.",
                Category = "Roster",
                Status = "Unread",
                CreatedDate = TimeZoneHelper.NowIst()
            });

            if (dto.NewUserId.HasValue && dto.NewUserId.Value != oldUserId)
            {
                _notifRepo.Add(new Notification
                {
                    UserId = dto.NewUserId.Value,
                    Message = $"You have been assigned a swapped shift on {entity.Date:yyyy-MM-dd}.",
                    Category = "Roster",
                    Status = "Unread",
                    CreatedDate = TimeZoneHelper.NowIst()
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Swapped",
                Resource = "RosterAssignment",
                Metadata = $"{{\"assignmentId\":{assignmentId},\"oldUserId\":{oldUserId},\"newUserId\":{entity.UserId}}}"
            });

            _db.SaveChanges();
            return MapAssignment(entity);
        }

        public void MarkAbsent(int assignmentId)
        {
            var entity = _assignRepo.GetById(assignmentId)
                ?? throw new KeyNotFoundException("Assignment not found.");
            if (entity.Status != "Assigned")
                throw new ArgumentException("Only Assigned shifts can be marked absent.");

            entity.Status = "Absent";
            _assignRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "MarkedAbsent",
                Resource = "RosterAssignment",
                Metadata = $"{{\"assignmentId\":{assignmentId}}}"
            });

            _db.SaveChanges();
        }

        public IEnumerable<RosterAssignmentResponseDto> SearchAssignments(RosterAssignmentSearchDto dto)
        {
            DateOnly? date = null;
            if (!string.IsNullOrWhiteSpace(dto.Date))
                date = ParseDate(dto.Date, "Date");

            return _assignRepo.Search(dto.SiteId, dto.UserId, date, dto.Status)
                .Select(MapAssignment).ToList();
        }

        // ===================== On-Call =====================

        public OnCallResponseDto CreateOnCall(CreateOnCallDto dto)
        {
            EnsureSiteActive(dto.SiteId);
            var date = ParseDate(dto.Date, "Date");
            if (date < TimeZoneHelper.TodayIst())
                throw new ArgumentException("On-call coverage cannot be created for a past date.");
            var start = ParseTime(dto.StartTime, "StartTime");
            var end = ParseTime(dto.EndTime, "EndTime");
            if (start >= end)
                throw new ArgumentException("StartTime must be before EndTime.");
            if (dto.BackupUserId.HasValue && dto.BackupUserId.Value == dto.PrimaryUserId)
                throw new ArgumentException("Backup user cannot be the same as primary user.");
            EnsureUserActive(dto.PrimaryUserId, "Primary user");
            if (dto.BackupUserId.HasValue && dto.BackupUserId.Value > 0)
                EnsureUserActive(dto.BackupUserId.Value, "Backup user");

            var overlaps = _onCallRepo.Search(dto.SiteId, date)
                .Any(o => o.Status == "Active" && o.StartTime < end && o.EndTime > start);
            if (overlaps)
                throw new ArgumentException("Overlapping on-call coverage exists for this site and date.");

            var entity = new OnCallCoverage
            {
                SiteId = dto.SiteId,
                Department = dto.Department,
                Date = date,
                StartTime = start,
                EndTime = end,
                PrimaryUserId = dto.PrimaryUserId,
                BackupUserId = dto.BackupUserId,
                Status = "Active"
            };

            _onCallRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Created",
                Resource = "OnCallCoverage",
                Metadata = $"{{\"siteId\":{dto.SiteId},\"date\":\"{date:yyyy-MM-dd}\"}}"
            });

            _db.SaveChanges();
            return MapOnCall(entity);
        }

        public OnCallResponseDto UpdateOnCall(int id, UpdateOnCallDto dto)
        {
            var entity = _onCallRepo.GetById(id)
                ?? throw new KeyNotFoundException("On-call coverage not found.");

            if (dto.Department != null) entity.Department = dto.Department;
            if (dto.Date != null) entity.Date = ParseDate(dto.Date, "Date");
            if (dto.StartTime != null) entity.StartTime = ParseTime(dto.StartTime, "StartTime");
            if (dto.EndTime != null) entity.EndTime = ParseTime(dto.EndTime, "EndTime");
            EnsureSiteActive(entity.SiteId);
            if (dto.PrimaryUserId.HasValue) entity.PrimaryUserId = dto.PrimaryUserId.Value;
            if (dto.BackupUserId.HasValue)
            {
                entity.BackupUserId = dto.BackupUserId.Value <= 0 ? null : dto.BackupUserId.Value;
            }
            if (dto.Status != null) entity.Status = dto.Status;
            if (entity.Date < TimeZoneHelper.TodayIst())
                throw new ArgumentException("On-call coverage cannot be updated for a past date.");

            if (entity.StartTime >= entity.EndTime)
                throw new ArgumentException("StartTime must be before EndTime.");
            if (entity.BackupUserId.HasValue && entity.BackupUserId.Value == entity.PrimaryUserId)
                throw new ArgumentException("Backup user cannot be the same as primary user.");
            EnsureUserActive(entity.PrimaryUserId, "Primary user");
            if (entity.BackupUserId.HasValue && entity.BackupUserId.Value > 0)
                EnsureUserActive(entity.BackupUserId.Value, "Backup user");

            _onCallRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Updated",
                Resource = "OnCallCoverage",
                Metadata = $"{{\"onCallId\":{id}}}"
            });

            _db.SaveChanges();
            return MapOnCall(entity);
        }

        public OnCallResponseDto GetOnCall(int id)
        {
            var entity = _onCallRepo.GetById(id)
                ?? throw new KeyNotFoundException("On-call coverage not found.");
            return MapOnCall(entity);
        }

        public IEnumerable<OnCallResponseDto> SearchOnCall(OnCallSearchDto dto)
        {
            DateOnly? date = null;
            if (!string.IsNullOrWhiteSpace(dto.Date))
                date = ParseDate(dto.Date, "Date");

            return _onCallRepo.Search(dto.SiteId, date)
                .Select(MapOnCall).ToList();
        }

        // ===================== Helpers =====================

        private static TimeOnly ParseTime(string value, string fieldName)
        {
            if (!TimeOnly.TryParseExact(value?.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                throw new ArgumentException($"Invalid {fieldName} format. Use HH:mm.");
            return result;
        }

        private static DateOnly ParseDate(string value, string fieldName)
        {
            if (!DateOnly.TryParseExact(value?.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                throw new ArgumentException($"Invalid {fieldName} format. Use yyyy-MM-dd.");
            return result;
        }

        private void EnsureSiteActive(int siteId)
        {
            var site = _siteRepo.Get(siteId) ?? throw new KeyNotFoundException($"Site {siteId} not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Site is not active.");
        }

        private void EnsureUserActive(int userId, string field)
        {
            var user = _userRepo.GetById(userId) ?? throw new KeyNotFoundException($"{field} not found.");
            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"{field} is not active.");
        }

        private static ShiftTemplateResponseDto MapShiftTemplate(ShiftTemplate e) => new()
        {
            ShiftTemplateId = e.ShiftTemplateId,
            Name = e.Name,
            StartTime = e.StartTime.ToString("HH:mm"),
            EndTime = e.EndTime.ToString("HH:mm"),
            BreakMinutes = e.BreakMinutes,
            Role = e.Role,
            SiteId = e.SiteId,
            Status = e.Status
        };

        private static RosterResponseDto MapRoster(Roster e) => new()
        {
            RosterId = e.RosterId,
            SiteId = e.SiteId,
            Department = e.Department,
            PeriodStart = e.PeriodStart.ToString("yyyy-MM-dd"),
            PeriodEnd = e.PeriodEnd.ToString("yyyy-MM-dd"),
            PublishedBy = e.PublishedBy,
            PublishedDate = e.PublishedDate,
            Status = e.Status
        };

        private static RosterAssignmentResponseDto MapAssignment(RosterAssignment e) => new()
        {
            AssignmentId = e.AssignmentId,
            RosterId = e.RosterId,
            UserId = e.UserId,
            ShiftTemplateId = e.ShiftTemplateId,
            Date = e.Date.ToString("yyyy-MM-dd"),
            Role = e.Role,
            Status = e.Status
        };

        private static OnCallResponseDto MapOnCall(OnCallCoverage e) => new()
        {
            OnCallId = e.OnCallId,
            SiteId = e.SiteId,
            Department = e.Department,
            Date = e.Date.ToString("yyyy-MM-dd"),
            StartTime = e.StartTime.ToString("HH:mm"),
            EndTime = e.EndTime.ToString("HH:mm"),
            StartDateTimeUtc = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, e.StartTime.Hour, e.StartTime.Minute, 0, DateTimeKind.Unspecified),
            EndDateTimeUtc = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, e.EndTime.Hour, e.EndTime.Minute, 0, DateTimeKind.Unspecified),
            PrimaryUserId = e.PrimaryUserId,
            BackupUserId = e.BackupUserId,
            Status = e.Status
        };
    }
}
