using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("slots")]
    [Authorize(Roles = "FrontDesk,Patient,Provider,Nurse,Tech,Admin")]
    public class SlotsController(IAvailabilityService _availabilityservice, ISlotGenerationService _slotGenerationService) : ControllerBase
    {
        // GET /slots?providerId=&serviceId=&siteId=&date=YYYY-MM-DD
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<SlotResponseDto>>> Get([FromQuery] int providerId, [FromQuery] int serviceId, [FromQuery] int siteId, [FromQuery] string date)
        {
            var data = _availabilityservice.GetOpenSlots(new SlotSearchRequestDto
            {
                ProviderId = providerId,
                ServiceId = serviceId,
                SiteId = siteId,
                Date = date
            });
            return ApiResponse<IEnumerable<SlotResponseDto>>.Ok(data, "Slots fetched.");
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<ProviderSlotGenerationResponseDto>> GenerateFromTemplate([FromBody] ProviderSlotGenerationRequestDto dto)
        {
            var isAdmin = User.IsAdmin();
            var providerId = User.GetProviderId();
            var result = _slotGenerationService.GenerateFromTemplate(dto, providerId, isAdmin);

            if (result.CancelledDueToConflict)
                return BadRequest(ApiResponse<ProviderSlotGenerationResponseDto>.Fail(
                    new { code = "SLOT_GENERATION_CONFLICT", conflicts = result.Conflicts },
                    "Slot generation cancelled due to conflicting already-generated slots."));

            return ApiResponse<ProviderSlotGenerationResponseDto>.Ok(result, "Slot generation completed.");
        }
    }
}