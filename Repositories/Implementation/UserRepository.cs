using System.Linq;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class UserRepository(CareScheduleContext _db) : IUserRepository
    {
        public User? GetByEmail(string email, string role)
        {
            // Email is UNIQUE by schema
            var e = (email ?? string.Empty).Trim();
            var r = (role  ?? string.Empty).Trim();

            // Compare case-insensitively in DB
            return _db.Users.FirstOrDefault(u =>
                u.Email == e &&
                u.Role.ToLower() == r.ToLower() &&
                u.Status == "Active"
            );
        }

        public User? GetById(int userId)
        {
            return _db.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public (List<User> Items, int Total) Search(
            string? name,
            string? role,
            string? email,
            string? phone,
            string? status,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir
            )
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var pattern = name.Trim();
                query = query.Where(u => EF.Functions.Like(u.Name, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var pattern = role.Trim();
                query = query.Where(u => EF.Functions.Like(u.Role, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var pattern = email.Trim();
                query = query.Where(u => EF.Functions.Like(u.Email, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var pattern = phone.Trim();
                query = query.Where(u => u.Phone != null && EF.Functions.Like(u.Phone, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(u => u.Status == s);
            }

            // Sorting
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name"   => desc ? query.OrderByDescending(u => u.Name)   : query.OrderBy(u => u.Name),
                "role"   => desc ? query.OrderByDescending(u => u.Role)   : query.OrderBy(u => u.Role),
                "email"  => desc ? query.OrderByDescending(u => u.Email)  : query.OrderBy(u => u.Email),
                "phone"  => desc ? query.OrderByDescending(u => u.Phone)  : query.OrderBy(u => u.Phone),
                "status" => desc ? query.OrderByDescending(u => u.Status) : query.OrderBy(u => u.Status),
                _        => query.OrderBy(u => u.Name)
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, total);
        }

        public User? Get(int id)
        {
            return _db.Users.AsNoTracking()
                .FirstOrDefault(u => u.UserId == id);
        }

        public User Create(User entity)
        {
            _db.Users.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(User entity)
        {
            _db.Users.Update(entity);
            _db.SaveChanges();
        }
    }
}