using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class HolidayRepository(CareScheduleContext _db) : IHolidayRepository
    {
        public (List<Holiday> Items, int Total) Search(
            int? siteId,
            DateOnly? date,
            DateOnly? from,
            DateOnly? to,
            string? status,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.Holidays.AsNoTracking().AsQueryable();

            if (siteId.HasValue)
            {
                var sid = siteId.Value;
                query = query.Where(h => h.SiteId == sid);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(h => h.Status == s);
            }

            if (date.HasValue)
            {
                var d = date.Value;
                query = query.Where(h => h.Date == d);
            }
            else
            {
                if (from.HasValue)
                {
                    var f = from.Value;
                    query = query.Where(h => h.Date >= f);
                }

                if (to.HasValue)
                {
                    var t = to.Value;
                    query = query.Where(h => h.Date <= t);
                }
            }

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "siteid"      => desc ? query.OrderByDescending(h => h.SiteId)      : query.OrderBy(h => h.SiteId),
                "status"      => desc ? query.OrderByDescending(h => h.Status)      : query.OrderBy(h => h.Status),
                "description" => desc ? query.OrderByDescending(h => h.Description) : query.OrderBy(h => h.Description),
                _             => desc ? query.OrderByDescending(h => h.Date)        : query.OrderBy(h => h.Date),
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, total);
        }

        public Holiday? Get(int id)
        {
            return _db.Holidays.AsNoTracking()
                .FirstOrDefault(h => h.HolidayId == id);
        }

        public Holiday? GetByDate(int siteId, DateOnly date)
        {
            return _db.Holidays.AsNoTracking()
                .FirstOrDefault(h => h.SiteId == siteId && h.Date == date);
        }

        public Holiday Create(Holiday entity)
        {
            _db.Holidays.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Holiday entity)
        {
            _db.Holidays.Update(entity);
            _db.SaveChanges();
        }
    }
}