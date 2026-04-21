using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
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
        [Authorize(Roles = "Admin,Provider")]
        [HttpPost("provider-services")]
        public IActionResult Assign([FromBody] ProviderServiceCreateDto dto)
        {
            if (!User.IsAdmin() && User.GetUserId() != dto.ProviderId)
                return StatusCode(403, ApiResponse<object>.Fail(
                    new { code = "ROLE_FORBIDDEN" }, "You can only manage your own provider-service mappings."));

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

        [Authorize(Roles = "Admin,Provider")]
        [HttpDelete("provider-services/{id:int}")]
        public IActionResult Remove(int id)
        {
            if (!User.IsAdmin())
            {
                var myProviderId = User.GetUserId();
                var mine = _mappingservice.GetServicesByProvider(myProviderId);
                if (!mine.Any(x => x.Psid == id))
                    return StatusCode(403, ApiResponse<object>.Fail(
                        new { code = "ROLE_FORBIDDEN" }, "You can only remove your own provider-service mappings."));
            }

            _mappingservice.RemoveMapping(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Mapping removed."));
        }
    }
}
