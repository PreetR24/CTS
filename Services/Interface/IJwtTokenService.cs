using CareSchedule.Models;

namespace CareSchedule.Services.Interface
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, int? siteId = null);
    }
}