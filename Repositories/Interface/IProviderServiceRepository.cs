using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IProviderServiceRepository
    {
        List<ProviderService> GetByProviderId(int providerId);
        List<ProviderService> GetByServiceId(int serviceId);
        ProviderService? GetByProviderAndService(int providerId, int serviceId);
        ProviderService? GetById(int psid);
        ProviderService Create(ProviderService entity);
        void Delete(ProviderService entity);
        IEnumerable<ProviderService> GetActiveByProvider(int providerId);
        bool AnyActiveByService(int serviceId);
    }
}
