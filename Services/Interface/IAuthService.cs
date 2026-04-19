using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IAuthService
    {
        LoginResponseDto Login(string email, string role);
        void Logout(int userId);
        MeResponseDto GetMe(int userId);
    }
}