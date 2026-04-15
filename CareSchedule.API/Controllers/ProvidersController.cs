using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/providers")]
    [Authorize]
    [Produces("application/json")]
    public class ProvidersController(IProviderMasterService _providerservice) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _providerservice.GetAllProviders();
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var provider = _providerservice.GetProvider(id);
            return provider is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Provider not found."))
                : Ok(ApiResponse<ProviderDto>.Ok(provider));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] ProviderCreateDto dto)
        {
            var created = _providerservice.CreateProvider(dto);
            return CreatedAtAction(nameof(Get), new { id = created.ProviderId },
                ApiResponse<ProviderDto>.Ok(created, "Provider created."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProviderUpdateDto dto)
        {
            var updated = _providerservice.UpdateProvider(id, dto);
            return Ok(ApiResponse<ProviderDto>.Ok(updated, "Provider updated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult Deactivate(int id)
        {
            _providerservice.DeactivateProvider(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Provider deactivated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/activate")]
        public IActionResult Activate(int id)
        {
            _providerservice.ActivateProvider(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Provider activated."));
        }
    }
}
