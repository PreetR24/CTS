using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("availability-templates")]
    [Authorize(Roles = "Provider,Admin")]
    public class AvailabilityTemplatesController(IAvailabilityService _availabilityservice) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<IdResponseDto>> Create([FromBody] CreateAvailabilityTemplateRequestDto dto)
        {
            var id = _availabilityservice.CreateTemplate(dto);
            return ApiResponse<IdResponseDto>.Ok(new IdResponseDto { Id = id }, "Template created.");
        }

        [HttpPut("{templateId:int}")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> Update(int templateId, [FromBody] UpdateAvailabilityTemplateRequestDto dto)
        {
            dto.TemplateId = templateId;
            _availabilityservice.UpdateTemplate(dto);
            return ApiResponse<object>.Ok(null, "Template updated.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<AvailabilityTemplateResponseDto>>> List([FromQuery] int providerId, [FromQuery] int siteId)
        {
            var data = _availabilityservice.ListTemplates(providerId, siteId);
            return ApiResponse<IEnumerable<AvailabilityTemplateResponseDto>>.Ok(data, "Templates fetched.");
        }
    }
}
