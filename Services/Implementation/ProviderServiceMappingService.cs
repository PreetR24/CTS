using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ProviderServiceMappingService(
            IProviderServiceRepository _psRepo,
            IProviderRepository _providerRepo,
            IServiceRepository _serviceRepo,
            IAuditLogService _auditService) : IProviderServiceMappingService
    {
        public ProviderServiceDto AssignServiceToProvider(ProviderServiceCreateDto dto)
        {
            var provider = _providerRepo.GetById(dto.ProviderId)
                ?? throw new ArgumentException($"Provider with ID {dto.ProviderId} does not exist.");

            var service = _serviceRepo.GetById(dto.ServiceId)
                ?? throw new ArgumentException($"Service with ID {dto.ServiceId} does not exist.");

            var existing = _psRepo.GetByProviderAndService(dto.ProviderId, dto.ServiceId);
            if (existing is not null)
                throw new ArgumentException(
                    $"Provider '{provider.Name}' is already mapped to Service '{service.Name}'.");

            var entity = new ProviderService
            {
                ProviderId = dto.ProviderId,
                ServiceId = dto.ServiceId,
                CustomDurationMin = dto.CustomDurationMin,
                CustomBufferBeforeMin = dto.CustomBufferBeforeMin,
                CustomBufferAfterMin = dto.CustomBufferAfterMin,
                Status = "Active"
            };

            entity = _psRepo.Create(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "AssignServiceToProvider",
                Resource = "ProviderService",
                Metadata = $"{{\"psid\":{entity.Psid},\"providerId\":{entity.ProviderId},\"serviceId\":{entity.ServiceId}}}"
            });

            return new ProviderServiceDto
            {
                Psid = entity.Psid,
                ProviderId = entity.ProviderId,
                ProviderName = provider.Name,
                ServiceId = entity.ServiceId,
                ServiceName = service.Name,
                CustomDurationMin = entity.CustomDurationMin,
                CustomBufferBeforeMin = entity.CustomBufferBeforeMin,
                CustomBufferAfterMin = entity.CustomBufferAfterMin,
                Status = entity.Status
            };
        }

        public List<ProviderServiceDto> GetServicesByProvider(int providerId)
        {
            var mappings = _psRepo.GetByProviderId(providerId);
            return mappings.Select(Map).ToList();
        }

        public List<ProviderServiceDto> GetProvidersByService(int serviceId)
        {
            var mappings = _psRepo.GetByServiceId(serviceId);
            return mappings.Select(Map).ToList();
        }

        public void RemoveMapping(int psid)
        {
            var entity = _psRepo.GetById(psid)
                ?? throw new KeyNotFoundException($"ProviderService mapping with ID {psid} not found.");

            _psRepo.Delete(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "RemoveProviderServiceMapping",
                Resource = "ProviderService",
                Metadata = $"{{\"psid\":{psid},\"providerId\":{entity.ProviderId},\"serviceId\":{entity.ServiceId}}}"
            });
        }

        private static ProviderServiceDto Map(ProviderService ps) => new()
        {
            Psid = ps.Psid,
            ProviderId = ps.ProviderId,
            ProviderName = ps.Provider.Name,
            ServiceId = ps.ServiceId,
            ServiceName = ps.Service.Name,
            CustomDurationMin = ps.CustomDurationMin,
            CustomBufferBeforeMin = ps.CustomBufferBeforeMin,
            CustomBufferAfterMin = ps.CustomBufferAfterMin,
            Status = ps.Status
        };
    }
}
