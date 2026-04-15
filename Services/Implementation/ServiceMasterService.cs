using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ServiceMasterService(
        IServiceRepository _servicerepo,
        IAuditLogService _auditService,
        IProviderServiceRepository _psRepo) : IServiceMasterService
    {
        private static readonly string[] ValidVisitTypes = { "New", "FollowUp", "Procedure" };

        public List<ServiceDto> GetAllServices()
        {
            var services = _servicerepo.GetAll();
            return services.Select(Map).ToList();
        }

        public ServiceDto? GetService(int id)
        {
            var service = _servicerepo.GetById(id);
            return service is null ? null : Map(service);
        }

        public ServiceDto CreateService(ServiceCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Service name is required.");

            if (string.IsNullOrWhiteSpace(dto.VisitType))
                throw new ArgumentException("VisitType is required.");

            if (!ValidVisitTypes.Contains(dto.VisitType))
                throw new ArgumentException($"VisitType must be one of: {string.Join(", ", ValidVisitTypes)}.");

            var entity = new Service
            {
                Name = dto.Name.Trim(),
                VisitType = dto.VisitType,
                DefaultDurationMin = dto.DefaultDurationMin,
                BufferBeforeMin = dto.BufferBeforeMin,
                BufferAfterMin = dto.BufferAfterMin,
                Status = "Active"
            };

            entity = _servicerepo.Create(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateService",
                Resource = "Service",
                Metadata = $"{{\"serviceId\":{entity.ServiceId},\"name\":\"{entity.Name}\"}}"
            });

            return Map(entity);
        }

        public ServiceDto UpdateService(int id, ServiceUpdateDto dto)
        {
            var entity = _servicerepo.GetById(id)
                ?? throw new KeyNotFoundException("Service not found.");

            if (dto.Name is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Service name cannot be empty.");
                entity.Name = dto.Name.Trim();
            }

            if (dto.VisitType is not null)
            {
                if (!ValidVisitTypes.Contains(dto.VisitType))
                    throw new ArgumentException($"VisitType must be one of: {string.Join(", ", ValidVisitTypes)}.");
                entity.VisitType = dto.VisitType;
            }

            if (dto.DefaultDurationMin.HasValue)
                entity.DefaultDurationMin = dto.DefaultDurationMin.Value;

            if (dto.BufferBeforeMin.HasValue)
                entity.BufferBeforeMin = dto.BufferBeforeMin.Value;

            if (dto.BufferAfterMin.HasValue)
                entity.BufferAfterMin = dto.BufferAfterMin.Value;

            _servicerepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateService",
                Resource = "Service",
                Metadata = $"{{\"serviceId\":{entity.ServiceId}}}"
            });

            return Map(entity);
        }

        public void DeactivateService(int id)
        {
            var entity = _servicerepo.GetById(id)
                ?? throw new KeyNotFoundException("Service not found.");

            if (entity.Status == "Inactive") return;

            if (_psRepo.AnyActiveByService(id))
                throw new InvalidOperationException("Cannot deactivate service: active provider-service mappings exist.");

            entity.Status = "Inactive";
            _servicerepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "DeactivateService",
                Resource = "Service",
                Metadata = $"{{\"serviceId\":{entity.ServiceId}}}"
            });
        }

        public void ActivateService(int id)
        {
            var entity = _servicerepo.GetById(id)
                ?? throw new KeyNotFoundException("Service not found.");

            if (entity.Status == "Active") return;

            entity.Status = "Active";
            _servicerepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "ActivateService",
                Resource = "Service",
                Metadata = $"{{\"serviceId\":{entity.ServiceId}}}"
            });
        }

        private static ServiceDto Map(Service s) => new()
        {
            ServiceId = s.ServiceId,
            Name = s.Name,
            VisitType = s.VisitType,
            DefaultDurationMin = s.DefaultDurationMin,
            BufferBeforeMin = s.BufferBeforeMin,
            BufferAfterMin = s.BufferAfterMin,
            Status = s.Status
        };
    }
}
