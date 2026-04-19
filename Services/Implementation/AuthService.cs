using System;
using System.Collections.Generic;
using CareSchedule.Services.Interface;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Services.Implementation
{
    public class AuthService(
        IUserRepository _userRepo,
        IAuditLogService _auditService,
        IJwtTokenService _jwtTokenService) : IAuthService
    {
        private static readonly string[] AllowedRoles =
        [
            "Patient", "FrontDesk", "Provider", "Nurse", "Operations", "Admin"
        ];

        public LoginResponseDto Login(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required.");

            var normEmail = email.Trim();
            var normRole = role.Trim();

            var matchedRole = AllowedRoles.FirstOrDefault(r =>
                string.Equals(r, normRole, StringComparison.OrdinalIgnoreCase));
            if (matchedRole == null)
                throw new ArgumentException("Invalid role.");

            var user = _userRepo.GetByEmail(normEmail, matchedRole);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User is inactive.");
            if (string.Equals(user.Status, "Locked", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User account is locked.");

            var token = _jwtTokenService.GenerateToken(user);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                UserId = user.UserId,
                Action = "Login",
                Resource = "User",
                Metadata = "{\"message\":\"User logged in\"}"
            });

            return new LoginResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Role = user.Role,
                Email = user.Email,
                Status = user.Status,
                ProviderId = user.ProviderId,
                Token = token
            };
        }

        public void Logout(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid userId.");

            var user = _userRepo.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                UserId = user.UserId,
                Action = "Logout",
                Resource = "User",
                Metadata = "{\"message\":\"User logged out\"}"
            });
        }

        public MeResponseDto GetMe(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid userId.");

            var user = _userRepo.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User is inactive.");
            if (string.Equals(user.Status, "Locked", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User account is locked.");

            return new MeResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Role = user.Role,
                Email = user.Email,
                LandingPage = ResolveLandingPage(user.Role)
            };
        }

        private static string ResolveLandingPage(string role) =>
            role switch
            {
                "Admin" => "/admin",
                "Provider" => "/provider",
                "FrontDesk" => "/frontdesk",
                "Nurse" => "/staff",
                "Tech" => "/staff",
                "Patient" => "/patient",
                "Operations" => "/operations",
                _ => "/"
            };

    }
}
