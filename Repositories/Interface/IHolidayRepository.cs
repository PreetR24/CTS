using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IHolidayRepository
    {
        (List<Holiday> Items, int Total) Search(
            int? siteId,
            DateOnly? date,
            DateOnly? from,
            DateOnly? to,
            string? status,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir);

        Holiday? Get(int id);

        Holiday? GetByDate(int siteId, DateOnly date);

        Holiday Create(Holiday entity);

        void Update(Holiday entity);
    }
}