using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;

namespace CareSchedule.Repositories.Implementation
{
    public class CalendarEventRepository(CareScheduleContext _db) : ICalendarEventRepository
    {
        public void Add(CalendarEvent entity)
        {
            _db.CalendarEvents.Add(entity);
        }

        public void DeleteByEntity(string entityType, int entityId)
        {
            // Projection cleanup: remove CalendarEvent rows for the given source entity
            var rows = _db.CalendarEvents
                .Where(e => e.EntityType == entityType && e.EntityId == entityId)
                .ToList();

            if (rows.Count > 0)
            {
                _db.CalendarEvents.RemoveRange(rows);
            }
        }

        public IEnumerable<CalendarEvent> ListBySiteDate(int siteId, DateTime date)
        {
            var d = date.Date;
            return _db.CalendarEvents
                .Where(e => e.SiteId == siteId && e.StartTime.Date == d)
                .OrderBy(e => e.StartTime)
                .ToList();
        }

        public IEnumerable<CalendarEvent> ListByProviderDate(int providerId, DateTime date)
        {
            var d = date.Date;
            return _db.CalendarEvents
                .Where(e => e.ProviderId == providerId && e.StartTime.Date == d)
                .OrderBy(e => e.StartTime)
                .ToList();
        }

        public CalendarEvent? GetById(int eventId)
        {
            return _db.CalendarEvents.FirstOrDefault(e => e.EventId == eventId);
        }

        public CalendarEvent? GetByEntity(string entityType, int entityId)
        {
            return _db.CalendarEvents
                .FirstOrDefault(e => e.EntityType == entityType && e.EntityId == entityId);
        }

        public void Update(CalendarEvent entity)
        {
            _db.CalendarEvents.Update(entity);
        }

        public void SetLatestEntityId(string entityType, int entityId)
        {
            var ev = _db.CalendarEvents
                .Where(e => e.EntityType == entityType && e.EntityId == 0)
                .OrderByDescending(e => e.EventId)
                .FirstOrDefault();

            if (ev != null)
            {
                ev.EntityId = entityId;
                _db.CalendarEvents.Update(ev);
            }
        }
    }
}