using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("waitlist")]
    [Authorize(Roles = "FrontDesk,Admin")]
    public class WaitlistController(IWaitlistService _waitlistservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<WaitlistResponseDto>> Add([FromBody] CreateWaitlistRequestDto dto)
        {
            var result = _waitlistservice.Add(dto);
            return ApiResponse<WaitlistResponseDto>.Ok(result, "Added to waitlist.");
        }

        [HttpDelete("{waitId:int}")]
        public ActionResult<ApiResponse<object>> Remove(int waitId)
        {
            _waitlistservice.Remove(waitId);
            return ApiResponse<object>.Ok(null, "Removed from waitlist.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<WaitlistResponseDto>>> Search([FromQuery] WaitlistSearchDto dto)
        {
            var list = _waitlistservice.Search(dto);
            return ApiResponse<IEnumerable<WaitlistResponseDto>>.Ok(list, "Waitlist fetched.");
        }

        [HttpPatch("{waitId:int}/filled")]
        public ActionResult<ApiResponse<WaitlistResponseDto>> Fill(int waitId, [FromBody] FillWaitlistRequestDto dto)
        {
            var result = _waitlistservice.Fill(waitId, dto);
            return ApiResponse<WaitlistResponseDto>.Ok(result, "Waitlist slot filled.");
        }
    }
}
