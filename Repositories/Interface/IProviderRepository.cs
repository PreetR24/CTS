using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IProviderRepository
    {
        List<Provider> GetAll();
        Provider? GetById(int id);
        Provider Create(Provider entity);
        Provider CreateWithId(Provider entity, int providerId);
        void Update(Provider entity);
    }
}
