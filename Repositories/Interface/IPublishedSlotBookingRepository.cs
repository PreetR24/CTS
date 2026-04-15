using System;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IPublishedSlotBookingRepository
    {
        PublishedSlot? GetById(int publishedSlotId);
        void Update(PublishedSlot entity);
        PublishedSlot? FindExact(int providerId, int siteId, DateOnly date, TimeOnly start, TimeOnly end);
    }
}