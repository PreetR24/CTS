using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("outcome")]
    [Authorize(Roles = "Provider,Admin")]
    public class OutcomesController(IOutcomeService _outcomeservice) : ControllerBase
    {
        [HttpPost("{appointmentId:int}")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> RecordOutcome(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _outcomeservice.RecordOutcome(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "Outcome recorded.");
        }

        [HttpPost("{appointmentId:int}/no-show")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> MarkNoShow(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _outcomeservice.MarkNoShow(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "No-show recorded.");
        }
    }
}
