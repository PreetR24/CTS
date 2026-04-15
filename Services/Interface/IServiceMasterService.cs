using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IServiceMasterService
    {
        List<ServiceDto> GetAllServices();
        ServiceDto? GetService(int id);
        ServiceDto CreateService(ServiceCreateDto dto);
        ServiceDto UpdateService(int id, ServiceUpdateDto dto);
        void DeactivateService(int id);
        void ActivateService(int id);
    }
}
