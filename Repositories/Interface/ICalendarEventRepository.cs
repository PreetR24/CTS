using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ICalendarEventRepository
    {
        void Add(CalendarEvent entity);

        // Optional cleanup when removing a block, etc.
        void DeleteByEntity(string entityType, int entityId); // e.g., ("Block", blockId)

        // Optional reads for diagnostics/admin
        IEnumerable<CalendarEvent> ListBySiteDate(int siteId, DateTime date);
        IEnumerable<CalendarEvent> ListByProviderDate(int providerId, DateTime date);
        CalendarEvent? GetById(int eventId);
        void SetLatestEntityId(string entityType, int entityId);
        CalendarEvent? GetByEntity(string entityType, int entityId);
        void Update(CalendarEvent entity);
    }
}