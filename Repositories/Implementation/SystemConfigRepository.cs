using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class SystemConfigRepository(CareScheduleContext _db) : ISystemConfigRepository
    {
        public (List<SystemConfig> Items, int Total) Search(
            string? key,
            string? scope,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.SystemConfigs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(key))
            {
                var pattern = key.Trim();
                query = query.Where(c => EF.Functions.Like(c.Key, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(scope))
            {
                var s = scope.Trim();
                query = query.Where(c => c.Scope == s);
            }

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "key"         => desc ? query.OrderByDescending(c => c.Key)         : query.OrderBy(c => c.Key),
                "value"       => desc ? query.OrderByDescending(c => c.Value)       : query.OrderBy(c => c.Value),
                "scope"       => desc ? query.OrderByDescending(c => c.Scope)       : query.OrderBy(c => c.Scope),
                "updatedby"   => desc ? query.OrderByDescending(c => c.UpdatedBy)   : query.OrderBy(c => c.UpdatedBy),
                "updateddate" => desc ? query.OrderByDescending(c => c.UpdatedDate) : query.OrderBy(c => c.UpdatedDate),
                _             => desc ? query.OrderByDescending(c => c.UpdatedDate) : query.OrderBy(c => c.UpdatedDate)
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, total);
        }

        public SystemConfig? Get(int id)
        {
            return _db.SystemConfigs.AsNoTracking()
                .FirstOrDefault(c => c.ConfigId == id);
        }

        public SystemConfig Create(SystemConfig entity)
        {
            _db.SystemConfigs.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(SystemConfig entity)
        {
            _db.SystemConfigs.Update(entity);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _db.SystemConfigs.Find(id);
            if (entity is null) return;
            _db.SystemConfigs.Remove(entity);
            _db.SaveChanges();
        }

        public int? GetInt(string key, int? defaultValue)
        {
            var row = _db.SystemConfigs.FirstOrDefault(x => x.Key == key);
            if (row == null) return defaultValue;
            if (int.TryParse(row.Value, out var v)) return v;
            return defaultValue;
        }
    }
}