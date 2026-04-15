using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class RoomRepository(CareScheduleContext _db) : IRoomRepository
    {
        public (List<Room> Items, int Total) Search(
            string? roomName,
            string? roomType,
            string? status,
            int? siteId,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.Rooms.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(roomName))
            {
                var pattern = roomName.Trim();
                query = query.Where(r => EF.Functions.Like(r.RoomName, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(roomType))
            {
                var pattern = roomType.Trim();
                // If your Room entity has RoomType column, this will work.
                // If not, remove this filter.
                query = query.Where(r => EF.Functions.Like(r.RoomType!, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(r => r.Status == s);
            }

            if (siteId.HasValue)
            {
                query = query.Where(r => r.SiteId == siteId.Value);
            }

            // Sorting (use lowercase keys)
            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "roomname" => desc ? query.OrderByDescending(r => r.RoomName) : query.OrderBy(r => r.RoomName),
                "roomtype" => desc ? query.OrderByDescending(r => r.RoomType) : query.OrderBy(r => r.RoomType),
                "status"   => desc ? query.OrderByDescending(r => r.Status)   : query.OrderBy(r => r.Status),
                "siteid"   => desc ? query.OrderByDescending(r => r.SiteId)   : query.OrderBy(r => r.SiteId),
                _          => query.OrderBy(r => r.RoomName)
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, total);
        }

        public Room? Get(int id)
        {
            return _db.Rooms.AsNoTracking()
                .FirstOrDefault(r => r.RoomId == id);
        }

        public Room Create(Room entity)
        {
            _db.Rooms.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Room entity)
        {
            _db.Rooms.Update(entity);
            _db.SaveChanges();
        }
    }
}