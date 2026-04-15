using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IProviderServiceMappingService
    {
        ProviderServiceDto AssignServiceToProvider(ProviderServiceCreateDto dto);
        List<ProviderServiceDto> GetServicesByProvider(int providerId);
        List<ProviderServiceDto> GetProvidersByService(int serviceId);
        void RemoveMapping(int psid);
    }
}
