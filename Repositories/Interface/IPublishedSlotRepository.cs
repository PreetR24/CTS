using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IPublishedSlotRepository
    {
        IEnumerable<PublishedSlot> GetOpenSlots(int providerId, int serviceId, int siteId, DateOnly date);

        void AddRange(IEnumerable<PublishedSlot> slots);
        PublishedSlot? GetById(int pubSlotId);
        void Update(PublishedSlot slot);

        IEnumerable<PublishedSlot> FindSlotsInWindow(
            int providerId,
            int siteId,
            DateOnly date,
            TimeOnly start,
            TimeOnly end,
            params string[] statuses
        );

        IEnumerable<PublishedSlot> FindBySiteDateRange(int siteId, DateOnly startDate, DateOnly endDate, params string[] statuses);
    }
}