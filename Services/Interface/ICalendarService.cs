using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ICalendarService
    {
        IEnumerable<CalendarEventResponseDto> GetByProvider(int providerId, string date);
        IEnumerable<CalendarEventResponseDto> GetBySite(int siteId, string date);
        CalendarEventResponseDto GetById(int eventId);
    }
}
