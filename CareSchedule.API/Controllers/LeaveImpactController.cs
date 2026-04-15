using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("leave-impact")]
    [Authorize(Roles = "Operations,Admin")]
    public class LeaveImpactController(ILeaveService _leaveservice) : ControllerBase
    {
        [HttpGet("leave/{leaveId:int}")]
        public ActionResult<ApiResponse<IEnumerable<LeaveImpactResponseDto>>> GetByLeaveId(int leaveId)
        {
            var list = _leaveservice.GetImpactsByLeaveId(leaveId);
            return ApiResponse<IEnumerable<LeaveImpactResponseDto>>.Ok(list, "Leave impacts fetched.");
        }

        [HttpGet("{impactId:int}")]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> GetById(int impactId)
        {
            var item = _leaveservice.GetImpactById(impactId);
            return ApiResponse<LeaveImpactResponseDto>.Ok(item, "Leave impact fetched.");
        }

        [HttpPost]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Create([FromBody] CreateLeaveImpactDto dto)
        {
            var result = _leaveservice.CreateImpact(dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact created.");
        }

        [HttpPatch("{impactId:int}/resolve")]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Resolve(int impactId, [FromBody] ResolveLeaveImpactDto dto)
        {
            var result = _leaveservice.ResolveImpact(impactId, dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact resolved.");
        }
    }
}
