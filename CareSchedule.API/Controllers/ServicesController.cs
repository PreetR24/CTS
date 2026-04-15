using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/services")]
    [Authorize]
    [Produces("application/json")]
    public class ServicesController(IServiceMasterService _service) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _service.GetAllServices();
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var svc = _service.GetService(id);
            return svc is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Service not found."))
                : Ok(ApiResponse<ServiceDto>.Ok(svc));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] ServiceCreateDto dto)
        {
            var created = _service.CreateService(dto);
            return CreatedAtAction(nameof(Get), new { id = created.ServiceId },
                ApiResponse<ServiceDto>.Ok(created, "Service created."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ServiceUpdateDto dto)
        {
            var updated = _service.UpdateService(id, dto);
            return Ok(ApiResponse<ServiceDto>.Ok(updated, "Service updated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult Deactivate(int id)
        {
            _service.DeactivateService(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Service deactivated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/activate")]
        public IActionResult Activate(int id)
        {
            _service.ActivateService(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Service activated."));
        }
    }
}
