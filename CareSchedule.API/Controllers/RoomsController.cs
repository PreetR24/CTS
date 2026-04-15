using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/rooms")]
    [Authorize]
    public class RoomsController(IRoomService _roomservice) : ControllerBase
    {
        [HttpGet]
        public IActionResult Search([FromQuery] RoomSearchQuery q)
            => Ok(ApiResponse<object>.Ok(_roomservice.SearchRoom(q)));

        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<RoomDto>> Get(int id)
            => Ok(ApiResponse<RoomDto>.Ok(_roomservice.GetRoom(id)));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<ApiResponse<RoomDto>> Create([FromBody] RoomCreateDto dto)
        {
            if (dto is null) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));
            var created = _roomservice.CreateRoom(dto);
            return CreatedAtAction(nameof(Get), new { id = created.RoomId }, ApiResponse<RoomDto>.Ok(created, "Room created."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<RoomDto>> Update(int id, [FromBody] RoomUpdateDto dto)
        {
            if (dto is null) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));
            return Ok(ApiResponse<RoomDto>.Ok(_roomservice.UpdateRoom(id, dto), "Room updated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int id)
        {
            _roomservice.DeactivateRoom(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Room deactivated."));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/activate")]
        public ActionResult<ApiResponse<object>> Activate(int id)
        {
            _roomservice.ActivateRoom(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Room activated."));
        }
    }
}
