using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("rosters")]
    [Authorize(Roles = "Operations,Admin")]
    public class RostersController(IRosterService _rosterservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<RosterResponseDto>>> Search([FromQuery] RosterSearchDto dto)
        {
            var list = _rosterservice.SearchRosters(dto);
            return ApiResponse<IEnumerable<RosterResponseDto>>.Ok(list, "Rosters fetched.");
        }

        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<RosterResponseDto>> GetById(int id)
        {
            var result = _rosterservice.GetRoster(id);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster fetched.");
        }

        [HttpPost]
        public ActionResult<ApiResponse<RosterResponseDto>> Create([FromBody] CreateRosterDto dto)
        {
            var result = _rosterservice.CreateRoster(dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster created.");
        }

        [HttpPut("{rosterId:int}")]
        public ActionResult<ApiResponse<RosterResponseDto>> Update(int rosterId, [FromBody] UpdateRosterDto dto)
        {
            var result = _rosterservice.UpdateRoster(rosterId, dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster updated.");
        }

        [HttpDelete("{rosterId:int}")]
        public ActionResult<ApiResponse<object>> Delete(int rosterId)
        {
            _rosterservice.DeleteRoster(rosterId);
            return ApiResponse<object>.Ok(new { id = rosterId }, "Roster deleted.");
        }

        [HttpPatch("{rosterId:int}/publish")]
        public ActionResult<ApiResponse<RosterResponseDto>> Publish(int rosterId, [FromBody] PublishRosterDto dto)
        {
            var result = _rosterservice.PublishRoster(rosterId, dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster published.");
        }
    }
}
