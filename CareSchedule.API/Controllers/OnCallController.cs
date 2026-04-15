using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("oncall")]
    [Authorize(Roles = "Operations,Admin")]
    public class OnCallController(IRosterService _rosterservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<OnCallResponseDto>>> Search([FromQuery] OnCallSearchDto dto)
        {
            var list = _rosterservice.SearchOnCall(dto);
            return ApiResponse<IEnumerable<OnCallResponseDto>>.Ok(list, "On-call coverages fetched.");
        }

        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<OnCallResponseDto>> GetById(int id)
        {
            var result = _rosterservice.GetOnCall(id);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage fetched.");
        }

        [HttpPost]
        public ActionResult<ApiResponse<OnCallResponseDto>> Create([FromBody] CreateOnCallDto dto)
        {
            var result = _rosterservice.CreateOnCall(dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage created.");
        }

        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<OnCallResponseDto>> Update(int id, [FromBody] UpdateOnCallDto dto)
        {
            var result = _rosterservice.UpdateOnCall(id, dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage updated.");
        }
    }
}
