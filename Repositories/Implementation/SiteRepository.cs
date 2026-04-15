using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class SiteRepository(CareScheduleContext _db) : ISiteRepository
    {
        public (List<Site> Items, int Total) Search(
            string? name, string? status,
            int page, int pageSize,
            string? sortBy, string? sortDir)
        {
            var q = _db.Sites.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var like = $"%{name.Trim().ToLower()}%";
                q = q.Where(s => EF.Functions.Like(s.Name.ToLower(), like));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                q = q.Where(s => s.Status == status);
            }

            var asc = !string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            q = (sortBy?.ToLower()) switch
            {
                "timezone" => asc ? q.OrderBy(x => x.Timezone) : q.OrderByDescending(x => x.Timezone),
                "status"   => asc ? q.OrderBy(x => x.Status)   : q.OrderByDescending(x => x.Status),
                _          => asc ? q.OrderBy(x => x.Name)     : q.OrderByDescending(x => x.Name),
            };

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            return (items, total);
        }

        public Site? Get(int id)
        {
            return _db.Sites
                .AsNoTracking()
                .FirstOrDefault(s => s.SiteId == id);
        }

        public Site Create(Site entity)
        {
            _db.Sites.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Site entity)
        {
            _db.Sites.Update(entity);
            _db.SaveChanges();
        }
    }
}