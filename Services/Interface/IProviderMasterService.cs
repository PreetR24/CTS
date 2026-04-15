using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IProviderMasterService
    {
        List<ProviderDto> GetAllProviders();
        ProviderDto? GetProvider(int id);
        ProviderDto CreateProvider(ProviderCreateDto dto);
        ProviderDto UpdateProvider(int id, ProviderUpdateDto dto);
        void DeactivateProvider(int id);
        void ActivateProvider(int id);
    }
}
