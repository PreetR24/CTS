using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class PublishedSlotRepository(CareScheduleContext _db) : IPublishedSlotRepository
    {
        public IEnumerable<PublishedSlot> GetOpenSlots(int providerId, int serviceId, int siteId, DateOnly date)
        {
            return _db.PublishedSlots
                .Where(s => s.ProviderId== providerId
                            && s.ServiceId== serviceId
                            && s.SiteId== siteId
                            && s.SlotDate == date
                            && s.Status == "Open")
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public void AddRange(IEnumerable<PublishedSlot> slots)
        {
            _db.PublishedSlots.AddRange(slots);
        }

        public PublishedSlot? GetById(int pubSlotId)
        {
            return _db.PublishedSlots.FirstOrDefault(s => s.PubSlotId== pubSlotId);
        }

        public void Update(PublishedSlot slot)
        {
            _db.PublishedSlots.Update(slot);
        }

        public IEnumerable<PublishedSlot> FindSlotsInWindow(
            int providerId,
            int siteId,
            DateOnly date,
            TimeOnly start,
            TimeOnly end,
            params string[] statuses)
        {
            var q = _db.PublishedSlots.Where(s =>
                    s.ProviderId== providerId
                    && s.SiteId== siteId
                    && s.SlotDate == date);

            if (statuses != null && statuses.Length > 0)
            {
                var set = statuses
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                q = q.Where(s => set.Contains(s.Status));
            }

            // Overlap: (s.Start < end) && (start < s.End)  -- all TimeOnly
            q = q.Where(s => s.StartTime < end && start < s.EndTime);

            return q.OrderBy(s => s.StartTime).ToList();
        }

        public IEnumerable<PublishedSlot> FindBySiteDateRange(int siteId, DateOnly startDate, DateOnly endDate, params string[] statuses)
        {
            var q = _db.PublishedSlots.Where(s => s.SiteId == siteId && s.SlotDate >= startDate && s.SlotDate <= endDate);

            if (statuses != null && statuses.Length > 0)
            {
                var set = statuses
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                q = q.Where(s => set.Contains(s.Status));
            }

            return q.OrderBy(s => s.SlotDate).ThenBy(s => s.StartTime).ToList();
        }
    }
}