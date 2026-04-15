using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IHolidayService
    {
        List<HolidayDto> SearchHoliday(HolidaySearchQuery query);

        HolidayDto GetHoliday(int id);

        HolidayDto GetHolidayByDate(int siteId, string date /* yyyy-MM-dd */);

        HolidayDto CreateHoliday(HolidayCreateDto dto);

        HolidayDto UpdateHoliday(int id, HolidayUpdateDto dto);

        void DeactivateHoliday(int id);

        void ActivateHoliday(int id);
    }
}