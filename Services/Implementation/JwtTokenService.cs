using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CareSchedule.Models;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class JwtTokenService(IConfiguration _configuration) : IJwtTokenService
    {
        public const string ClaimSiteId = "SiteId";
        public const string ClaimProviderId = "ProviderId";

        public string GenerateToken(User user, int? siteId = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role)
            };

            if (siteId.HasValue)
                claims.Add(new Claim(ClaimSiteId, siteId.Value.ToString()));

            if (user.ProviderId.HasValue)
                claims.Add(new Claim(ClaimProviderId, user.ProviderId.Value.ToString()));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiryMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}