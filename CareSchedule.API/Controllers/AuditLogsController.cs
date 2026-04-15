using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/iam/auditlogs")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController(IAuditLogService _auditservice) : ControllerBase
    {
        [HttpGet]
        public IActionResult Search([FromQuery] AuditLogSearchQuery q)
            => Ok(ApiResponse<object>.Ok(_auditservice.SearchAudit(q)));

        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<AuditLogDto>> Get(int id)
            => Ok(ApiResponse<AuditLogDto>.Ok(_auditservice.GetAudit(id)));

        [HttpPost]
        public ActionResult<ApiResponse<AuditLogDto>> Create([FromBody] AuditLogCreateDto dto)
        {
            if (dto is null) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));
            var created = _auditservice.CreateAudit(dto);
            return CreatedAtAction(nameof(Get), new { id = created.AuditId }, ApiResponse<AuditLogDto>.Ok(created, "Audit log created."));
        }
    }
}