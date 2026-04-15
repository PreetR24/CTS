using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/admin/holidays")]
    [Authorize]
    [Produces("application/json")]
    public class HolidaysController(IHolidayService _holidayservice) : ControllerBase
    {
        [HttpGet]
        public IActionResult Search([FromQuery] HolidaySearchQuery query)
        {
            var items = _holidayservice.SearchHoliday(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<HolidayDto>> Get(int id)
        {
            var holiday = _holidayservice.GetHoliday(id);
            return Ok(ApiResponse<HolidayDto>.Ok(holiday));
        }

        [HttpGet("by-date/{siteId:int}/{date}")]
        public ActionResult<ApiResponse<HolidayDto>> GetByDate(int siteId, string date)
        {
            var holiday = _holidayservice.GetHolidayByDate(siteId, date);
            return Ok(ApiResponse<HolidayDto>.Ok(holiday));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<ApiResponse<HolidayDto>> Create([FromBody] HolidayCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            var created = _holidayservice.CreateHoliday(dto);
            return CreatedAtAction(nameof(Get), new { id = created.HolidayId },
                ApiResponse<HolidayDto>.Ok(created, "Holiday created."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<HolidayDto>> Update(int id, [FromBody] HolidayUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            var updated = _holidayservice.UpdateHoliday(id, dto);
            return Ok(ApiResponse<HolidayDto>.Ok(updated, "Holiday updated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int id)
        {
            _holidayservice.DeactivateHoliday(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Holiday deactivated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/activate")]
        public ActionResult<ApiResponse<object>> Activate(int id)
        {
            _holidayservice.ActivateHoliday(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Holiday activated."));
        }
    }
}
