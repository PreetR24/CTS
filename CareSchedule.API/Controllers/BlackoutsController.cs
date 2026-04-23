using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("blackouts")]
    [Authorize(Roles = "Admin")]
    public class BlackoutsController(IBlackoutService _blackoutService) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<BlackoutResponseDto>> Create([FromBody] CreateBlackoutRequestDto dto)
        {
            var result = _blackoutService.Create(dto);
            return ApiResponse<BlackoutResponseDto>.Ok(result, "Blackout created.");
        }

        [HttpDelete("{blackoutId:int}")]
        public ActionResult<ApiResponse<object>> Cancel(int blackoutId)
        {
            _blackoutService.Cancel(blackoutId);
            return ApiResponse<object>.Ok(null, "Blackout cancelled.");
        }

        [HttpPut("{blackoutId:int}/activate")]
        public ActionResult<ApiResponse<object>> Activate(int blackoutId)
        {
            _blackoutService.Activate(blackoutId);
            return ApiResponse<object>.Ok(null, "Blackout activated.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<BlackoutResponseDto>>> List(
            [FromQuery] int siteId,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate)
        {
            var data = _blackoutService.List(siteId, startDate, endDate);
            return ApiResponse<IEnumerable<BlackoutResponseDto>>.Ok(data, "Blackouts fetched.");
        }
    }
}
