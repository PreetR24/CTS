using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ISystemConfigRepository
    {
        (List<SystemConfig> Items, int Total) Search(
            string? key,
            string? scope,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir);

        SystemConfig? Get(int id);
        SystemConfig Create(SystemConfig entity);
        void Update(SystemConfig entity);
        void Delete(int id);
        int? GetInt(string key, int? defaultValue);
    }
}