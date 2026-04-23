using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("outcome")]
    [Authorize(Roles = "Provider,Admin,Patient")]
    public class OutcomesController(IOutcomeService _outcomeservice, IAppointmentRepository _appointmentRepo) : ControllerBase
    {
        [HttpGet("{appointmentId:int}")]
        public ActionResult<ApiResponse<OutcomeResponseDto?>> GetByAppointment(int appointmentId)
        {
            var appointment = _appointmentRepo.GetById(appointmentId);
            if (appointment == null)
                return NotFound(ApiResponse<OutcomeResponseDto?>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Appointment not found."));

            var role = User.GetRole();
            var userId = User.GetUserId();
            if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase) && appointment.PatientId != userId)
                return Forbid();
            if (string.Equals(role, "Provider", StringComparison.OrdinalIgnoreCase) && appointment.ProviderId != userId)
                return Forbid();

            var result = _outcomeservice.GetOutcomeByAppointment(appointmentId);
            return ApiResponse<OutcomeResponseDto?>.Ok(result, result == null ? "Outcome not available yet." : "Outcome fetched.");
        }

        [HttpPost("{appointmentId:int}")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> RecordOutcome(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _outcomeservice.RecordOutcome(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "Outcome recorded.");
        }
    }
}
