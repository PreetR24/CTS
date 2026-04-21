using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("leave")]
    [Authorize]
    public class LeaveController(ILeaveService _leaveservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Submit([FromBody] CreateLeaveRequestDto dto)
        {
            var userId = User.GetUserId();
            var result = _leaveservice.Submit(userId, dto);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave request submitted.");
        }

        [HttpGet("{leaveId:int}")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> GetById(int leaveId)
        {
            var result = _leaveservice.GetById(leaveId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave request fetched.");
        }

        [HttpGet]
        [Authorize]
        public ActionResult<ApiResponse<IEnumerable<LeaveRequestResponseDto>>> Search([FromQuery] LeaveSearchDto dto)
        {
            if (!User.IsAdmin() && !string.Equals(User.GetRole(), "Operations", StringComparison.OrdinalIgnoreCase))
            {
                dto.UserId = User.GetUserId();
            }
            var list = _leaveservice.Search(dto);
            return ApiResponse<IEnumerable<LeaveRequestResponseDto>>.Ok(list, "Leave requests fetched.");
        }

        [HttpPatch("{leaveId:int}/cancel")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Cancel(int leaveId)
        {
            var userId = User.GetUserId();
            var result = _leaveservice.Cancel(leaveId, userId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave request cancelled.");
        }

        [HttpPatch("{leaveId:int}/approve")]
        [Authorize(Roles = "Operations,Admin")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Approve(int leaveId)
        {
            var result = _leaveservice.Approve(leaveId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave approved.");
        }

        [HttpPatch("{leaveId:int}/reject")]
        [Authorize(Roles = "Operations,Admin")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Reject(int leaveId)
        {
            var result = _leaveservice.Reject(leaveId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave rejected.");
        }

        [HttpGet("{leaveId:int}/impacts")]
        public ActionResult<ApiResponse<IEnumerable<LeaveImpactResponseDto>>> GetImpacts(int leaveId)
        {
            var leave = _leaveservice.GetById(leaveId);
            if (!User.IsAdmin() &&
                !string.Equals(User.GetRole(), "Operations", StringComparison.OrdinalIgnoreCase) &&
                leave.UserId != User.GetUserId())
            {
                return StatusCode(403, ApiResponse<object>.Fail(
                    new { code = "ROLE_FORBIDDEN" }, "You can only view impacts for your own leave."));
            }

            var list = _leaveservice.GetImpactsByLeaveId(leaveId);
            return ApiResponse<IEnumerable<LeaveImpactResponseDto>>.Ok(list, "Leave impacts fetched.");
        }
    }
}
