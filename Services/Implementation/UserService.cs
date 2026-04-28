using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace CareSchedule.Services.Implementation
{
    public class UserService(IUserRepository _userrepo, IProviderRepository _providerRepo) : IUserService
    {
        public List<UserDto> SearchUser(UserSearchQuery q)
        {
            var (items, _) = _userrepo.Search(q.Name, q.Role, q.Email, q.Phone, q.Status,
                                          q.Page, q.PageSize, q.SortBy, q.SortDir);
            var list = new List<UserDto>(items.Count);
            foreach (var u in items) list.Add(Map(u));
            return list;
        }

        public UserDto GetUser(int id)
        {
            var u = _userrepo.Get(id);
            if (u is null) throw new KeyNotFoundException("User not found.");
            return Map(u);
        }

        public UserDto CreateUser(UserCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Role)) throw new ArgumentException("Role is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required.");
            if (dto.Name.Trim().Length < 2) throw new ArgumentException("Name must be at least 2 characters.");
            if (!Regex.IsMatch(dto.Name.Trim(), @"^[A-Za-z][A-Za-z\s'.-]*$"))
                throw new ArgumentException("Name contains invalid characters.");
            if (!Regex.IsMatch(dto.Email.Trim(), @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new ArgumentException("Enter a valid email address.");
            if (!string.IsNullOrWhiteSpace(dto.Phone) && !Regex.IsMatch(dto.Phone.Trim(), @"^\d{10}$"))
                throw new ArgumentException("Phone number must be exactly 10 digits.");
            if (string.Equals(dto.Role, "Provider", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.Specialty))
                    throw new ArgumentException("Specialty is required for Provider.");
                if (string.IsNullOrWhiteSpace(dto.Credentials))
                    throw new ArgumentException("Credentials are required for Provider.");
            }

            var e = new User
            {
                Name = dto.Name.Trim(),
                Role = dto.Role.Trim(),
                Email = dto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                Status = "Active"
            };
            e = _userrepo.Create(e);

            // If a provider user is created, create a Provider row using the same numeric ID as UserId.
            if (string.Equals(e.Role, "Provider", StringComparison.OrdinalIgnoreCase))
            {
                var provider = new Provider
                {
                    Name = e.Name,
                    Specialty = string.IsNullOrWhiteSpace(dto.Specialty) ? null : dto.Specialty.Trim(),
                    Credentials = string.IsNullOrWhiteSpace(dto.Credentials) ? null : dto.Credentials.Trim(),
                    ContactInfo = e.Phone,
                    Status = "Active"
                };

                _providerRepo.CreateWithId(provider, e.UserId);
            }

            return Map(e);
        }

        public UserDto UpdateUser(int id, UserUpdateDto dto)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Trim().Length < 2) throw new ArgumentException("Name must be at least 2 characters.");
                if (!Regex.IsMatch(dto.Name.Trim(), @"^[A-Za-z][A-Za-z\s'.-]*$"))
                    throw new ArgumentException("Name contains invalid characters.");
                e.Name = dto.Name.Trim();
            }
            if (!string.IsNullOrWhiteSpace(dto.Role)) e.Role = dto.Role.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!Regex.IsMatch(dto.Email.Trim(), @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                    throw new ArgumentException("Enter a valid email address.");
                e.Email = dto.Email.Trim();
            }
            if (dto.Phone is not null)
            {
                if (!string.IsNullOrWhiteSpace(dto.Phone) && !Regex.IsMatch(dto.Phone.Trim(), @"^\d{10}$"))
                    throw new ArgumentException("Phone number must be exactly 10 digits.");
                e.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            }
            _userrepo.Update(e);
            return Map(e);
        }

        public void DeactivateUser(int id)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");
            if (e.Status != "Inactive") { e.Status = "Inactive"; _userrepo.Update(e); }
        }

        public void ActivateUser(int id)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");
            if (e.Status != "Active") { e.Status = "Active"; _userrepo.Update(e); }
        }

        public async Task<MeResponseDto> GetMeAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid userId.");

            var user = await _userrepo.GetByIdAsync(userId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("User is inactive.");
            return new MeResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Role = user.Role,
                Email = user.Email,
                Phone = user.Phone,
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
                "Patient" => "/patient",
                "Operations" => "/operations",
                _ => "/"
            };
        
        private static UserDto Map(User u) => new()
        {
            UserId = u.UserId,
            Name = u.Name,
            Role = u.Role,
            Email = u.Email,
            Phone = u.Phone,
            Status = u.Status
        };
    }
}