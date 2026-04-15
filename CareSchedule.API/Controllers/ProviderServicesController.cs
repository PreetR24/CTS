using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata")]
    [Authorize]
    [Produces("application/json")]
    public class ProviderServicesController(IProviderServiceMappingService _mappingservice) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("provider-services")]
        public IActionResult Assign([FromBody] ProviderServiceCreateDto dto)
        {
            var created = _mappingservice.AssignServiceToProvider(dto);
            return CreatedAtAction(nameof(GetServicesByProvider), new { providerId = created.ProviderId },
                ApiResponse<ProviderServiceDto>.Ok(created, "Service assigned to provider."));
        }

        [HttpGet("providers/{providerId:int}/services")]
        public IActionResult GetServicesByProvider(int providerId)
        {
            var items = _mappingservice.GetServicesByProvider(providerId);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("services/{serviceId:int}/providers")]
        public IActionResult GetProvidersByService(int serviceId)
        {
            var items = _mappingservice.GetProvidersByService(serviceId);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("provider-services/{id:int}")]
        public IActionResult Remove(int id)
        {
            _mappingservice.RemoveMapping(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Mapping removed."));
        }
    }
}
