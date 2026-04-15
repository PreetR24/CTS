using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("sla")]
    [Authorize(Roles = "Admin")]
    public class SlaController(IRulesService _ruleservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<SlaResponseDto>>> Search(
            [FromQuery] string? scope, [FromQuery] string? status)
        {
            var list = _ruleservice.SearchSlas(scope, status);
            return ApiResponse<IEnumerable<SlaResponseDto>>.Ok(list, "SLAs fetched.");
        }

        [HttpGet("{slaId:int}")]
        public ActionResult<ApiResponse<SlaResponseDto>> Get(int slaId)
        {
            var result = _ruleservice.GetSla(slaId);
            return ApiResponse<SlaResponseDto>.Ok(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse<SlaResponseDto>> Create([FromBody] CreateSlaDto dto)
        {
            var result = _ruleservice.CreateSla(dto);
            return ApiResponse<SlaResponseDto>.Ok(result, "SLA created.");
        }

        [HttpPut("{slaId:int}")]
        public ActionResult<ApiResponse<SlaResponseDto>> Update(int slaId, [FromBody] UpdateSlaDto dto)
        {
            var result = _ruleservice.UpdateSla(slaId, dto);
            return ApiResponse<SlaResponseDto>.Ok(result, "SLA updated.");
        }

        [HttpDelete("{slaId:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int slaId)
        {
            _ruleservice.DeactivateSla(slaId);
            return ApiResponse<object>.Ok(new { slaId }, "SLA deactivated.");
        }
    }
}