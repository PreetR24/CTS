using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("reports")]
    [Authorize(Roles = "Operations,Admin")]
    public class ReportsController(IReportService _reportservice) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<OpsReportResponseDto>> GetById(int id)
        {
            var item = _reportservice.GetById(id);
            return ApiResponse<OpsReportResponseDto>.Ok(item, "Report fetched.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<OpsReportResponseDto>>> Search([FromQuery] ReportSearchDto dto)
        {
            var list = _reportservice.Search(dto);
            return ApiResponse<IEnumerable<OpsReportResponseDto>>.Ok(list, "Reports fetched.");
        }

        [HttpPost]
        public ActionResult<ApiResponse<OpsReportResponseDto>> Create([FromBody] CreateReportDto dto)
        {
            var item = _reportservice.Create(dto);
            return ApiResponse<OpsReportResponseDto>.Ok(item, "Report created.");
        }

        [HttpGet("export")]
        public IActionResult Export([FromQuery] ReportSearchDto dto)
        {
            var bytes = _reportservice.Export(dto);
            return File(bytes, "application/octet-stream", "report.csv");
        }
    }
}
