using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("shift-templates")]
    [Authorize]
    public class ShiftTemplatesController(IRosterService _shifttemplateservice) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Operations,Admin,Nurse,FrontDesk")]
        public ActionResult<ApiResponse<IEnumerable<ShiftTemplateResponseDto>>> Search([FromQuery] ShiftTemplateSearchDto dto)
        {
            var list = _shifttemplateservice.SearchShiftTemplates(dto);
            return ApiResponse<IEnumerable<ShiftTemplateResponseDto>>.Ok(list, "Shift templates fetched.");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Operations,Admin,Nurse,FrontDesk")]
        public ActionResult<ApiResponse<ShiftTemplateResponseDto>> GetById(int id)
        {
            var result = _shifttemplateservice.GetShiftTemplate(id);
            return ApiResponse<ShiftTemplateResponseDto>.Ok(result, "Shift template fetched.");
        }

        [HttpPost]
        [Authorize(Roles = "Operations,Admin")]
        public ActionResult<ApiResponse<ShiftTemplateResponseDto>> Create([FromBody] CreateShiftTemplateDto dto)
        {
            var result = _shifttemplateservice.CreateShiftTemplate(dto);
            return ApiResponse<ShiftTemplateResponseDto>.Ok(result, "Shift template created.");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Operations,Admin")]
        public ActionResult<ApiResponse<ShiftTemplateResponseDto>> Update(int id, [FromBody] UpdateShiftTemplateDto dto)
        {
            var result = _shifttemplateservice.UpdateShiftTemplate(id, dto);
            return ApiResponse<ShiftTemplateResponseDto>.Ok(result, "Shift template updated.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Operations,Admin")]
        public ActionResult<ApiResponse<object>> Delete(int id)
        {
            _shifttemplateservice.DeleteShiftTemplate(id);
            return ApiResponse<object>.Ok(null, "Shift template deleted.");
        }
    }
}
