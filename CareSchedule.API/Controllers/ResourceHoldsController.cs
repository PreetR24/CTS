using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("resource-holds")]
    [Authorize(Roles = "FrontDesk,Admin")]
    public class ResourceHoldsController(IResourceHoldService _resourceholdservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ResourceHoldResponseDto>>> Search(
            [FromQuery] int? siteId,
            [FromQuery] string? resourceType,
            [FromQuery] int? resourceId)
        {
            var list = _resourceholdservice.Search(siteId, resourceType, resourceId);
            return ApiResponse<IEnumerable<ResourceHoldResponseDto>>.Ok(list, "Resource holds fetched.");
        }

        [HttpGet("{holdId:int}")]
        public ActionResult<ApiResponse<ResourceHoldResponseDto>> Get(int holdId)
        {
            var result = _resourceholdservice.GetById(holdId);
            return ApiResponse<ResourceHoldResponseDto>.Ok(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse<ResourceHoldResponseDto>> Create([FromBody] ResourceHoldCreateDto dto)
        {
            var result = _resourceholdservice.Create(dto);
            return ApiResponse<ResourceHoldResponseDto>.Ok(result, "Resource hold created.");
        }

        [HttpPut("{holdId:int}")]
        public ActionResult<ApiResponse<ResourceHoldResponseDto>> Update(int holdId, [FromBody] ResourceHoldUpdateDto dto)
        {
            var result = _resourceholdservice.Update(holdId, dto);
            return ApiResponse<ResourceHoldResponseDto>.Ok(result, "Resource hold updated.");
        }

        [HttpDelete("{holdId:int}")]
        public ActionResult<ApiResponse<object>> Release(int holdId)
        {
            _resourceholdservice.Release(holdId);
            return ApiResponse<object>.Ok(new { holdId }, "Resource hold released.");
        }
    }
}