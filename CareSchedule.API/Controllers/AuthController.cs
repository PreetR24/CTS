using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.Services.Interface;
using CareSchedule.DTOs;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService _authService, IUserService _userService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var result = _authService.Login(dto.Email, dto.Role);
            return ApiResponse<LoginResponseDto>.Ok(result, "Login successful.");
        }

        [AllowAnonymous]
        [HttpPost("signup/patient")]
        public ActionResult<ApiResponse<UserDto>> SignupPatient([FromBody] PatientSignupDto dto)
        {
            var created = _userService.CreateUser(new UserCreateDto
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Patient"
            });
            return ApiResponse<UserDto>.Ok(created, "Patient signup successful.");
        }

        [Authorize]
        [HttpPost("logout")]
        public ActionResult<ApiResponse<object>> Logout()
        {
            var userId = User.GetUserId();
            _authService.Logout(userId);
            return ApiResponse<object>.Ok(null, "Logout successful.");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<MeResponseDto>>> Me()
        {
            var userId = User.GetUserId();
            var result = await _userService.GetMeAsync(userId);
            return ApiResponse<MeResponseDto>.Ok(result, "User details fetched.");
        }

    }
}
