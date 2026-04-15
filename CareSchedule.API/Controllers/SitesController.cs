using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/sites")]
    [Authorize]
    public class SitesController(ISiteService _siteservice) : ControllerBase
    {
        [HttpGet]
        public IActionResult Search([FromQuery] SiteSearchQuery query)
        {
            var items = _siteservice.SearchSite(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var site = _siteservice.GetSite(id);
            return site is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."))
                : Ok(ApiResponse<SiteDto>.Ok(site));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] SiteCreateDto dto)
        {
            var created = _siteservice.CreateSite(dto);
            return CreatedAtAction(nameof(Get), new { id = created.SiteId },
                ApiResponse<SiteDto>.Ok(created, "Site created."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] SiteUpdateDto dto)
        {
            var updated = _siteservice.UpdateSite(id, dto);
            return Ok(ApiResponse<SiteDto>.Ok(updated, "Site updated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult DeactivateSite(int id)
        {
            _siteservice.DeactivateSite(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Site deactivated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/activate")]
        public IActionResult ActivateSite(int id)
        {
            _siteservice.ActivateSite(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Site activated."));
        }
    }
}
