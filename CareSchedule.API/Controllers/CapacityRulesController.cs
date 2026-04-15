using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("capacity-rules")]
    [Authorize(Roles = "Admin")]
    public class CapacityRulesController(IRulesService _ruleservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CapacityRuleResponseDto>>> Search(
            [FromQuery] string? scope, [FromQuery] string? status)
        {
            var list = _ruleservice.SearchCapacityRules(scope, status);
            return ApiResponse<IEnumerable<CapacityRuleResponseDto>>.Ok(list, "Capacity rules fetched.");
        }

        [HttpGet("{ruleId:int}")]
        public ActionResult<ApiResponse<CapacityRuleResponseDto>> Get(int ruleId)
        {
            var result = _ruleservice.GetCapacityRule(ruleId);
            return ApiResponse<CapacityRuleResponseDto>.Ok(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse<CapacityRuleResponseDto>> Create([FromBody] CreateCapacityRuleDto dto)
        {
            var result = _ruleservice.CreateCapacityRule(dto);
            return ApiResponse<CapacityRuleResponseDto>.Ok(result, "Capacity rule created.");
        }

        [HttpPut("{ruleId:int}")]
        public ActionResult<ApiResponse<CapacityRuleResponseDto>> Update(int ruleId, [FromBody] UpdateCapacityRuleDto dto)
        {
            var result = _ruleservice.UpdateCapacityRule(ruleId, dto);
            return ApiResponse<CapacityRuleResponseDto>.Ok(result, "Capacity rule updated.");
        }

        [HttpDelete("{ruleId:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int ruleId)
        {
            _ruleservice.DeactivateCapacityRule(ruleId);
            return ApiResponse<object>.Ok(new { ruleId }, "Capacity rule deactivated.");
        }
    }
}