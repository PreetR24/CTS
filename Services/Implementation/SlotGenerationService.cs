using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class SlotGenerationService(
        IAvailabilityTemplateRepository _templateRepo,
        IAvailabilityBlockRepository _blockRepo,
        IPublishedSlotRepository _slotRepo,
        ISiteRepository _siteRepo,
        IProviderRepository _providerRepo,
        IProviderServiceRepository _providerServiceRepo,
        IHolidayRepository _holidayRepo,
        IBlackoutRepository _blackoutRepo,
        IAuditLogService _auditService,
        CareScheduleContext _db) : ISlotGenerationService
    {
        public ProviderSlotGenerationResponseDto GenerateFromTemplate(ProviderSlotGenerationRequestDto dto, int? currentProviderId, bool isAdmin)
        {
            if (dto.TemplateId <= 0) throw new ArgumentException("TemplateId is required.");
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");
            if (dto.Days <= 0) throw new ArgumentException("Days must be positive.");

            EnsureSiteActive(dto.SiteId);

            var template = _templateRepo.GetById(dto.TemplateId)
                ?? throw new KeyNotFoundException("Template not found.");

            if (!string.Equals(template.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Selected template is not active.");
            if (template.SiteId != dto.SiteId)
                throw new ArgumentException("Template does not belong to this site.");

            EnsureProviderActive(template.ProviderId);

            if (!isAdmin)
            {
                if (!currentProviderId.HasValue)
                    throw new ArgumentException("Provider identity is missing in token.");
                if (template.ProviderId != currentProviderId.Value)
                    throw new ArgumentException("You can only generate slots from your own template.");
            }

            var candidates = BuildCandidates(template, dto.Days);
            if (candidates.Count == 0)
            {
                return new ProviderSlotGenerationResponseDto
                {
                    InsertedCount = 0,
                    SkippedExistingCount = 0,
                    CancelledDueToConflict = false
                };
            }

            var conflicts = FindConflicts(dto.TemplateId, candidates);
            var conflictingKeys = conflicts
                .Select(c => $"{c.ProviderId}|{c.SiteId}|{c.Date}|{c.StartTime}|{c.EndTime}")
                .ToHashSet(StringComparer.Ordinal);
            var insertableCandidates = candidates
                .Where(c => !conflictingKeys.Contains(
                    $"{c.ProviderId}|{c.SiteId}|{c.SlotDate:yyyy-MM-dd}|{c.StartTime:HH\\:mm}|{c.EndTime:HH\\:mm}"))
                .ToList();

            _slotRepo.AddRange(insertableCandidates.Select(c => new PublishedSlot
            {
                ProviderId = c.ProviderId,
                SiteId = c.SiteId,
                ServiceId = c.ServiceId,
                SlotDate = c.SlotDate,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                Status = "Open"
            }));

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "GenerateSlotsFromTemplate",
                Resource = "PublishedSlot",
                Metadata = System.Text.Json.JsonSerializer.Serialize(new
                {
                    dto.TemplateId,
                    dto.SiteId,
                    dto.Days,
                    inserted = insertableCandidates.Count,
                    skippedExisting = conflicts.Count
                })
            });
            _db.SaveChanges();

            return new ProviderSlotGenerationResponseDto
            {
                InsertedCount = insertableCandidates.Count,
                SkippedExistingCount = conflicts.Count,
                CancelledDueToConflict = false
            };
        }

        private List<SlotGenerationConflictDto> FindConflicts(int templateId, List<SlotCandidate> candidates)
        {
            var conflicts = new List<SlotGenerationConflictDto>();
            if (candidates.Count == 0) return conflicts;

            var providerId = candidates[0].ProviderId;
            var siteId = candidates[0].SiteId;
            var minDate = candidates.Min(c => c.SlotDate);
            var maxDate = candidates.Max(c => c.SlotDate);

            // Load all potentially conflicting slots once, then resolve overlaps in-memory.
            var existingSlots = _slotRepo
                .FindBySiteDateRange(siteId, minDate, maxDate, "Open", "Held", "Closed")
                .Where(s => s.ProviderId == providerId)
                .ToList();
            var existingByDate = existingSlots
                .GroupBy(s => s.SlotDate)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.StartTime).ToList());

            foreach (var c in candidates)
            {
                if (!existingByDate.TryGetValue(c.SlotDate, out var existingForDay)) continue;
                var existing = existingForDay.FirstOrDefault(s => s.StartTime < c.EndTime && c.StartTime < s.EndTime);
                if (existing == null) continue;

                conflicts.Add(new SlotGenerationConflictDto
                {
                    TemplateId = templateId,
                    ProviderId = c.ProviderId,
                    SiteId = c.SiteId,
                    Date = c.SlotDate.ToString("yyyy-MM-dd"),
                    StartTime = c.StartTime.ToString("HH:mm"),
                    EndTime = c.EndTime.ToString("HH:mm"),
                    ExistingSlotId = existing.PubSlotId,
                    ExistingStatus = existing.Status
                });
            }

            return conflicts;
        }

        private List<SlotCandidate> BuildCandidates(AvailabilityTemplate template, int days)
        {
            var candidates = new List<SlotCandidate>();
            var today = TimeZoneHelper.TodayIst();
            var endDate = today.AddDays(days - 1);

            var services = _providerServiceRepo.GetActiveByProvider(template.ProviderId).ToList();
            if (services.Count == 0)
                throw new ArgumentException("Provider has no active service mappings.");

            var holidays = _holidayRepo.Search(template.SiteId, null, today, endDate, "Active", 1, 9999, null, null).Items;
            var holidayDates = new HashSet<DateOnly>(holidays.Select(h => h.Date));
            var blackouts = _blackoutRepo.ListBySiteDateRange(template.SiteId, today, endDate).ToList();

            for (var offset = 0; offset < days; offset++)
            {
                var day = today.AddDays(offset);
                if ((int)day.DayOfWeek != template.DayOfWeek) continue;
                if (holidayDates.Contains(day)) continue;
                if (blackouts.Any(b => b.StartDate <= day && day <= b.EndDate)) continue;
                var activeBlocksForDay = _blockRepo.List(template.ProviderId, template.SiteId, day)
                    .Where(b => string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var providerService in services)
                {
                    var current = template.StartTime;
                    while (current < template.EndTime)
                    {
                        var next = current.AddMinutes(template.SlotDurationMin);
                        if (next > template.EndTime) break;
                        var blocked = activeBlocksForDay.Any(b => b.StartTime < next && current < b.EndTime);
                        if (blocked)
                        {
                            current = next;
                            continue;
                        }

                        candidates.Add(new SlotCandidate
                        {
                            ProviderId = template.ProviderId,
                            SiteId = template.SiteId,
                            ServiceId = providerService.ServiceId,
                            SlotDate = day,
                            StartTime = current,
                            EndTime = next
                        });

                        current = next;
                    }
                }
            }

            return candidates;
        }

        private void EnsureSiteActive(int siteId)
        {
            var site = _siteRepo.Get(siteId);
            if (site == null) throw new KeyNotFoundException("Site not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Site is not active.");
        }

        private void EnsureProviderActive(int providerId)
        {
            var provider = _providerRepo.GetById(providerId);
            if (provider == null) throw new KeyNotFoundException("Provider not found.");
            if (!string.Equals(provider.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Provider is not active.");
        }

        private sealed class SlotCandidate
        {
            public int ProviderId { get; set; }
            public int SiteId { get; set; }
            public int ServiceId { get; set; }
            public DateOnly SlotDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
        }
    }
}
