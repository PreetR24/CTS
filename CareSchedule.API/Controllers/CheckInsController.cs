using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "FrontDesk,Nurse")]
    public class CheckInsController(ICheckInService _checkinservice) : ControllerBase
    {
        [HttpPost("checkin/{appointmentId:int}")]
        [Authorize(Roles = "FrontDesk")]
        public ActionResult<ApiResponse<CheckInResponseDto>> CheckIn(int appointmentId, [FromBody] CreateCheckInRequestDto dto)
        {
            var result = _checkinservice.CheckIn(appointmentId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Patient checked in.");
        }

        [HttpPatch("checkins/{checkInId:int}/assign-room")]
        [Authorize(Roles = "FrontDesk")]
        public ActionResult<ApiResponse<CheckInResponseDto>> AssignRoom(int checkInId, [FromBody] AssignRoomRequestDto dto)
        {
            var result = _checkinservice.AssignRoom(checkInId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Room assigned.");
        }

        [HttpPatch("checkins/{checkInId:int}/in-room")]
        [Authorize(Roles = "Nurse")]
        public ActionResult<ApiResponse<CheckInResponseDto>> MoveToRoom(int checkInId)
        {
            var result = _checkinservice.MoveToRoom(checkInId);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Patient moved to room.");
        }

        [HttpPatch("checkins/{checkInId:int}/status")]
        [Authorize(Roles = "Nurse")]
        public ActionResult<ApiResponse<CheckInResponseDto>> UpdateStatus(int checkInId, [FromBody] UpdateCheckInStatusDto dto)
        {
            var result = _checkinservice.UpdateStatus(checkInId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Check-in status updated.");
        }

        [HttpPatch("checkins/{checkInId:int}/with-provider")]
        [Authorize(Roles = "Nurse")]
        public ActionResult<ApiResponse<CheckInResponseDto>> StartConsultation(int checkInId)
        {
            var result = _checkinservice.StartConsultation(checkInId);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Consultation started.");
        }

        [HttpGet("checkins")]
        public ActionResult<ApiResponse<IEnumerable<CheckInResponseDto>>> Search([FromQuery] CheckInSearchDto dto)
        {
            var list = _checkinservice.Search(dto);
            return ApiResponse<IEnumerable<CheckInResponseDto>>.Ok(list, "Check-ins fetched.");
        }
    }
}
