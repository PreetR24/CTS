using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IServiceRepository
    {
        List<Service> GetAll();
        Service? GetById(int id);
        Service Create(Service entity);
        void Update(Service entity);
    }
}
