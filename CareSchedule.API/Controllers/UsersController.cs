using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/iam/users")]
[Authorize]
    public class UsersController(IUserService _userservice) : ControllerBase
    {
        [HttpGet]
    [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] UserSearchQuery q)
            => Ok(ApiResponse<object>.Ok(_userservice.SearchUser(q)));

        [HttpGet("{id:int}")]
        [Authorize]
        public ActionResult<ApiResponse<UserDto>> Get(int id)
        {
            if (!User.IsAdmin() && User.GetUserId() != id)
                return StatusCode(403, ApiResponse<object>.Fail(
                    new { code = "ROLE_FORBIDDEN" }, "You can only view your own profile."));

            return Ok(ApiResponse<UserDto>.Ok(_userservice.GetUser(id)));
        }

        [HttpPost]
    [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<UserDto>> Create([FromBody] UserCreateDto dto)
        {
            if (dto is null) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));
            var created = _userservice.CreateUser(dto);
            return CreatedAtAction(nameof(Get), new { id = created.UserId }, ApiResponse<UserDto>.Ok(created, "User created."));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<UserDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (dto is null) return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

        if (!User.IsAdmin())
        {
            if (User.GetUserId() != id)
                return StatusCode(403, ApiResponse<object>.Fail(
                    new { code = "ROLE_FORBIDDEN" }, "You can only update your own profile."));

            var callerRole = User.GetRole();
            if (string.IsNullOrWhiteSpace(dto.RequesterRole) ||
                !string.Equals(dto.RequesterRole, callerRole, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, ApiResponse<object>.Fail(
                    new { code = "ROLE_FORBIDDEN" }, "Role mismatch for self profile update."));

            // Self-update cannot change role.
            dto.Role = null;
        }

            return Ok(ApiResponse<UserDto>.Ok(_userservice.UpdateUser(id, dto), "User updated."));
        }

        [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<object>> Deactivate(int id)
        {
            _userservice.DeactivateUser(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "User deactivated."));
        }

        [HttpPost("{id:int}/activate")]
    [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<object>> Activate(int id)
        {
            _userservice.ActivateUser(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "User activated."));
        }

    }
}
