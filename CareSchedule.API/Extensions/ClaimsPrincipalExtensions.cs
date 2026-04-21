using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CareSchedule.Services.Implementation;

namespace CareSchedule.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException("Invalid token: missing user ID.");
        }

        public static string GetRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Role)
                ?? throw new UnauthorizedAccessException("Invalid token: missing role.");
        }

        public static int? GetSiteId(this ClaimsPrincipal principal)
        {
            var val = principal.FindFirstValue(JwtTokenService.ClaimSiteId);
            return int.TryParse(val, out var id) ? id : null;
        }

        public static int? GetProviderId(this ClaimsPrincipal principal)
        {
            var val = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? principal.FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedAccessException("Invalid token: missing email.");
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return string.Equals(principal.GetRole(), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSelf(this ClaimsPrincipal principal, int userId)
        {
            return principal.GetUserId() == userId;
        }
    }
}
