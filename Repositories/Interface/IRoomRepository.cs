using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IRoomRepository
    {
        (List<Room> Items, int Total) Search(
            string? roomName,
            string? roomType,
            string? status,
            int? siteId,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir);

        Room? Get(int id);

        Room Create(Room entity);

        void Update(Room entity);
    }
}