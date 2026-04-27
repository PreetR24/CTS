using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class CalendarService(ICalendarEventRepository _calendarRepo) : ICalendarService
    {
        public CalendarEventResponseDto GetById(int eventId)
        {
            if (eventId <= 0) throw new ArgumentException("Invalid eventId.");
            var entity = _calendarRepo.GetById(eventId);
            if (entity == null) throw new KeyNotFoundException("Calendar event not found.");
            return Map(entity);
        }

        public IEnumerable<CalendarEventResponseDto> GetByProvider(int providerId, string date)
        {
            if (providerId <= 0)
                throw new ArgumentException("ProviderId is required.");

            var d = ParseDate(date);
            var items = _calendarRepo.ListByProviderDate(providerId, d);
            return items.Select(Map).ToList();
        }

        public IEnumerable<CalendarEventResponseDto> GetBySite(int siteId, string date)
        {
            if (siteId <= 0)
                throw new ArgumentException("SiteId is required.");

            var d = ParseDate(date);
            var items = _calendarRepo.ListBySiteDate(siteId, d);
            return items.Select(Map).ToList();
        }

        private static CalendarEventResponseDto Map(CalendarEvent e) => new()
        {
            EventId = e.EventId,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            ProviderId = e.ProviderId,
            SiteId = e.SiteId,
            RoomId = e.RoomId,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Status = e.Status
        };

        private static DateTime ParseDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return TimeZoneHelper.NowIst();

            if (!DateTime.TryParseExact(date.Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");

            return parsed;
        }
    }
}