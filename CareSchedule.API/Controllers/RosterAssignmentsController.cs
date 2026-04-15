using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("roster-assignments")]
    [Authorize(Roles = "Operations,Admin")]
    public class RosterAssignmentsController(IRosterService _rosterassignmentservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<RosterAssignmentResponseDto>> Assign([FromBody] CreateRosterAssignmentDto dto)
        {
            var result = _rosterassignmentservice.AssignStaff(dto);
            return ApiResponse<RosterAssignmentResponseDto>.Ok(result, "Staff assigned.");
        }

        [HttpPatch("{id:int}/swap")]
        public ActionResult<ApiResponse<RosterAssignmentResponseDto>> Swap(int id, [FromBody] SwapAssignmentDto dto)
        {
            var result = _rosterassignmentservice.SwapShift(id, dto);
            return ApiResponse<RosterAssignmentResponseDto>.Ok(result, "Shift swapped.");
        }

        [HttpPatch("{id:int}/absent")]
        public ActionResult<ApiResponse<object>> MarkAbsent(int id)
        {
            _rosterassignmentservice.MarkAbsent(id);
            return ApiResponse<object>.Ok(new { id }, "Marked absent.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<RosterAssignmentResponseDto>>> Search([FromQuery] RosterAssignmentSearchDto dto)
        {
            var list = _rosterassignmentservice.SearchAssignments(dto);
            return ApiResponse<IEnumerable<RosterAssignmentResponseDto>>.Ok(list, "Roster assignments fetched.");
        }
    }
}
