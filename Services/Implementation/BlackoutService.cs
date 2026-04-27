using System.Globalization;
using System.Text.Json;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class BlackoutService(
        IBlackoutRepository _blackoutRepo,
        IPublishedSlotRepository _slotRepo,
        ICalendarEventRepository _calendarRepo,
        ISiteRepository _siteRepo,
        IAuditLogService _auditService,
        CareScheduleContext _db) : IBlackoutService
    {
        public BlackoutResponseDto Create(CreateBlackoutRequestDto dto)
        {
            EnsureSiteActive(dto.SiteId);

            if (string.IsNullOrWhiteSpace(dto.StartDate)) throw new ArgumentException("StartDate is required.");
            if (string.IsNullOrWhiteSpace(dto.EndDate)) throw new ArgumentException("EndDate is required.");

            var startDate = ParseDateOnly(dto.StartDate);
            var endDate = ParseDateOnly(dto.EndDate);
            if (endDate < startDate) throw new ArgumentException("EndDate must be on or after StartDate.");

            var hasDuplicate = _blackoutRepo
                .ListBySite(dto.SiteId)
                .Any(b =>
                    string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                    b.StartDate == startDate &&
                    b.EndDate == endDate);
            if (hasDuplicate)
                throw new ArgumentException("Duplicate blackout already exists for this site and date range.");

            var entity = new Blackout
            {
                SiteId = dto.SiteId,
                StartDate = startDate,
                EndDate = endDate,
                Reason = dto.Reason?.Trim(),
                Status = "Active"
            };

            _blackoutRepo.Add(entity);
            _db.SaveChanges();

            var slotsToClose = _slotRepo.FindBySiteDateRange(dto.SiteId, startDate, endDate, "Open", "Held");
            foreach (var s in slotsToClose)
            {
                s.Status = "Closed";
                _slotRepo.Update(s);
            }

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                _calendarRepo.Add(new CalendarEvent
                {
                    EntityType = "Blackout",
                    EntityId = entity.BlackoutId,
                    ProviderId = null,
                    SiteId = dto.SiteId,
                    RoomId = null,
                    StartTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0, DateTimeKind.Unspecified),
                    EndTime = new DateTime(day.Year, day.Month, day.Day, 23, 59, 0, DateTimeKind.Unspecified),
                    Status = "Active"
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateBlackout",
                Resource = "Blackout",
                Metadata = JsonSerializer.Serialize(new
                {
                    entity.BlackoutId,
                    entity.SiteId,
                    StartDate = startDate.ToString("yyyy-MM-dd"),
                    EndDate = endDate.ToString("yyyy-MM-dd"),
                    ClosedSlots = slotsToClose.Count()
                })
            });

            _db.SaveChanges();
            return Map(entity);
        }

        public void Cancel(int blackoutId)
        {
            if (blackoutId <= 0) throw new ArgumentException("Invalid blackoutId.");

            var entity = _blackoutRepo.GetById(blackoutId) ?? throw new KeyNotFoundException("Blackout not found.");
            if (entity.Status == "Cancelled") return;

            entity.Status = "Cancelled";
            _blackoutRepo.Update(entity);
            _calendarRepo.DeleteByEntity("Blackout", blackoutId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CancelBlackout",
                Resource = "Blackout",
                Metadata = JsonSerializer.Serialize(new { blackoutId })
            });

            _db.SaveChanges();
        }

        public void Activate(int blackoutId)
        {
            if (blackoutId <= 0) throw new ArgumentException("Invalid blackoutId.");

            var entity = _blackoutRepo.GetById(blackoutId) ?? throw new KeyNotFoundException("Blackout not found.");
            EnsureSiteActive(entity.SiteId);
            if (string.Equals(entity.Status, "Active", StringComparison.OrdinalIgnoreCase)) return;

            var hasDuplicate = _blackoutRepo
                .ListBySite(entity.SiteId)
                .Any(b =>
                    b.BlackoutId != entity.BlackoutId &&
                    string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                    b.StartDate == entity.StartDate &&
                    b.EndDate == entity.EndDate);
            if (hasDuplicate)
                throw new ArgumentException("Another active blackout already exists for this site and date range.");

            entity.Status = "Active";
            _blackoutRepo.Update(entity);

            for (var day = entity.StartDate; day <= entity.EndDate; day = day.AddDays(1))
            {
                _calendarRepo.Add(new CalendarEvent
                {
                    EntityType = "Blackout",
                    EntityId = entity.BlackoutId,
                    ProviderId = null,
                    SiteId = entity.SiteId,
                    RoomId = null,
                    StartTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0, DateTimeKind.Unspecified),
                    EndTime = new DateTime(day.Year, day.Month, day.Day, 23, 59, 0, DateTimeKind.Unspecified),
                    Status = "Active"
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "ActivateBlackout",
                Resource = "Blackout",
                Metadata = JsonSerializer.Serialize(new { blackoutId })
            });

            _db.SaveChanges();
        }

        public IEnumerable<BlackoutResponseDto> List(int siteId, string? startDate, string? endDate)
        {
            EnsureSiteActive(siteId);

            IEnumerable<Blackout> items;
            if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
            {
                var sd = ParseDateOnly(startDate);
                var ed = ParseDateOnly(endDate);
                items = _blackoutRepo.ListBySiteDateRange(siteId, sd, ed);
            }
            else
            {
                items = _blackoutRepo.ListBySite(siteId);
            }

            return items.Select(Map).ToList();
        }

        private void EnsureSiteActive(int siteId)
        {
            if (siteId <= 0) throw new ArgumentException("Invalid siteId.");
            var site = _siteRepo.Get(siteId);
            if (site == null) throw new KeyNotFoundException("Site not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Site is not active.");
        }

        private static DateOnly ParseDateOnly(string value)
        {
            if (!DateOnly.TryParseExact(value.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
            return parsed;
        }

        private static BlackoutResponseDto Map(Blackout b) => new()
        {
            BlackoutId = b.BlackoutId,
            SiteId = b.SiteId,
            StartDate = b.StartDate.ToString("yyyy-MM-dd"),
            EndDate = b.EndDate.ToString("yyyy-MM-dd"),
            Reason = b.Reason,
            Status = b.Status
        };
    }
}
