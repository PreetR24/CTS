using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ISiteRepository
    {
        (List<Site> Items, int Total) Search(
            string? name, string? status,
            int page, int pageSize,
            string sortBy, string sortDir);

        Site? Get(int id);
        Site Create(Site entity);
        void Update(Site entity);
    }
}