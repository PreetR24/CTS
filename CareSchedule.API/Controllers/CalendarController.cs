using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("calendar")]
    [Authorize(Roles = "FrontDesk,Provider,Nurse,Admin")]
    public class CalendarController(ICalendarService _calenderservice) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<CalendarEventResponseDto>> GetById(int id)
        {
            var item = _calenderservice.GetById(id);
            return ApiResponse<CalendarEventResponseDto>.Ok(item, "Calendar event fetched.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CalendarEventResponseDto>>> Get(
            [FromQuery] int? providerId,
            [FromQuery] int? siteId,
            [FromQuery] string? date)
        {
            if (providerId.HasValue)
            {
                var list = _calenderservice.GetByProvider(providerId.Value, date ?? "");
                return ApiResponse<IEnumerable<CalendarEventResponseDto>>.Ok(list, "Calendar events fetched.");
            }

            if (siteId.HasValue)
            {
                var list = _calenderservice.GetBySite(siteId.Value, date ?? "");
                return ApiResponse<IEnumerable<CalendarEventResponseDto>>.Ok(list, "Calendar events fetched.");
            }

            return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "providerId or siteId is required."));
        }
    }
}
