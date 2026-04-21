using System.Collections.Generic;
using System.Threading.Tasks;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IUserService
    {
        List<UserDto> SearchUser(UserSearchQuery query);
        UserDto GetUser(int id);
        UserDto CreateUser(UserCreateDto dto);
        UserDto UpdateUser(int id, UserUpdateDto dto);
        void DeactivateUser(int id);
        void ActivateUser(int id);
        void LockUser(int id);
        void UnlockUser(int id);
        Task<MeResponseDto> GetMeAsync(int userId);
    }
}