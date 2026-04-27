using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("availability-blocks")]
    [Authorize(Roles = "Provider,Admin")]
    public class AvailabilityBlocksController(IAvailabilityService _availabilityservice) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<IdResponseDto>> Create([FromBody] CreateAvailabilityBlockRequestDto dto)
        {
            var id = _availabilityservice.CreateBlock(dto);
            return ApiResponse<IdResponseDto>.Ok(new IdResponseDto { Id = id }, "Block created.");
        }

        [HttpPut("{blockId:int}")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> Update(int blockId, [FromBody] CreateAvailabilityBlockRequestDto dto)
        {
            _availabilityservice.UpdateBlock(blockId, dto);
            return ApiResponse<object>.Ok(null, "Block updated.");
        }

        [HttpDelete("{blockId:int}")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> Delete(int blockId)
        {
            _availabilityservice.RemoveBlock(blockId);
            return ApiResponse<object>.Ok(null, "Block removed.");
        }

        [HttpPatch("{blockId:int}/activate")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> Activate(int blockId)
        {
            _availabilityservice.ActivateBlock(blockId);
            return ApiResponse<object>.Ok(null, "Block activated.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<AvailabilityBlockResponseDto>>> List([FromQuery] int providerId, [FromQuery] int siteId, [FromQuery] string? date)
        {
            var data = _availabilityservice.ListBlocks(providerId, siteId, date);
            return ApiResponse<IEnumerable<AvailabilityBlockResponseDto>>.Ok(data, "Blocks fetched.");
        }
    }
}
