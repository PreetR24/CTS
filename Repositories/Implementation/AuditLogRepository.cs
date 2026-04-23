using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class AuditLogRepository(CareScheduleContext _db) : IAuditLogRepository
    {
        public (List<AuditLog> Items, int Total) Search(
            int? userId,
            string? userName,
            string? action,
            string? resource,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.AuditLogs.AsNoTracking().AsQueryable();

            if (userId.HasValue)
            {
                var id = userId.Value;
                query = query.Where(a => a.UserId == id);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var pattern = userName.Trim();
                query = query.Where(a =>
                    a.UserId.HasValue &&
                    _db.Users.Any(u => u.UserId == a.UserId.Value && EF.Functions.Like(u.Name, $"%{pattern}%")));
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                var pattern = action.Trim();
                query = query.Where(a => EF.Functions.Like(a.Action, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(resource))
            {
                var pattern = resource.Trim();
                query = query.Where(a => EF.Functions.Like(a.Resource, $"%{pattern}%"));
            }

            if (from.HasValue)
            {
                var f = from.Value;
                query = query.Where(a => a.Timestamp >= f);
            }

            if (to.HasValue)
            {
                var t = to.Value;
                query = query.Where(a => a.Timestamp <= t);
            }

            // Sorting
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "action"   => desc ? query.OrderByDescending(a => a.Action)   : query.OrderBy(a => a.Action),
                "resource" => desc ? query.OrderByDescending(a => a.Resource) : query.OrderBy(a => a.Resource),
                "userid"   => desc ? query.OrderByDescending(a => a.UserId)   : query.OrderBy(a => a.UserId),
                _          => desc ? query.OrderByDescending(a => a.Timestamp) : query.OrderBy(a => a.Timestamp)
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, total);
        }

        public AuditLog? Get(int id)
        {
            return _db.AuditLogs.AsNoTracking()
                .FirstOrDefault(a => a.AuditId == id);
        }

        public AuditLog Create(AuditLog entity)
        {
            _db.AuditLogs.Add(entity);
            _db.SaveChanges();
            return entity;
        }
    }
}