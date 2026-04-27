using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class ProviderMasterService(
        IProviderRepository _providerrepo,
        IUserRepository _userRepo,
        IAuditLogService _auditService,
        IAvailabilityTemplateRepository _templateRepo,
        IAppointmentRepository _appointmentRepo) : IProviderMasterService
    {
        public List<ProviderDto> GetAllProviders()
        {
            var providers = _providerrepo.GetAll();
            return providers.Select(Map).ToList();
        }

        public ProviderDto? GetProvider(int id)
        {
            var provider = _providerrepo.GetById(id);
            return provider is null ? null : Map(provider);
        }

        public ProviderDto CreateProvider(ProviderCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Provider name is required.");

            var email = string.IsNullOrWhiteSpace(dto.Email)
                ? BuildGeneratedProviderEmail(dto.Name)
                : dto.Email.Trim();
            var contactInfo = string.IsNullOrWhiteSpace(dto.ContactInfo)
                ? email
                : dto.ContactInfo.Trim();

            var providerUser = _userRepo.Create(new User
            {
                Name = dto.Name.Trim(),
                Role = "Provider",
                Email = email,
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                Status = "Active"
            });

            var entity = new Provider
            {
                ProviderId = providerUser.UserId,
                Name = dto.Name.Trim(),
                Specialty = dto.Specialty?.Trim(),
                Credentials = dto.Credentials?.Trim(),
                ContactInfo = contactInfo,
                Status = "Active"
            };

            entity = _providerrepo.CreateWithId(entity, providerUser.UserId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateProvider",
                Resource = "Provider",
                Metadata = $"{{\"providerId\":{entity.ProviderId},\"name\":\"{entity.Name}\"}}"
            });

            return Map(entity);
        }

        public ProviderDto UpdateProvider(int id, ProviderUpdateDto dto)
        {
            var entity = _providerrepo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (dto.Name is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Provider name cannot be empty.");
                entity.Name = dto.Name.Trim();
            }

            if (dto.Specialty is not null)
                entity.Specialty = dto.Specialty.Trim();

            if (dto.Credentials is not null)
                entity.Credentials = dto.Credentials.Trim();

            if (dto.ContactInfo is not null)
                entity.ContactInfo = dto.ContactInfo.Trim();

            _providerrepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateProvider",
                Resource = "Provider",
                Metadata = $"{{\"providerId\":{entity.ProviderId}}}"
            });

            return Map(entity);
        }

        public void DeactivateProvider(int id)
        {
            var entity = _providerrepo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (entity.Status == "Inactive") return;

            if (_templateRepo.AnyActiveByProvider(id))
                throw new InvalidOperationException("Cannot deactivate provider: active availability templates exist.");

            var upcomingAppts = _appointmentRepo.Search(null, id, null, null, "Booked");
            if (upcomingAppts.Any())
                throw new InvalidOperationException("Cannot deactivate provider: upcoming booked appointments exist.");

            entity.Status = "Inactive";
            _providerrepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "DeactivateProvider",
                Resource = "Provider",
                Metadata = $"{{\"providerId\":{entity.ProviderId}}}"
            });
        }

        public void ActivateProvider(int id)
        {
            var entity = _providerrepo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (entity.Status == "Active") return;

            entity.Status = "Active";
            _providerrepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "ActivateProvider",
                Resource = "Provider",
                Metadata = $"{{\"providerId\":{entity.ProviderId}}}"
            });
        }

        private static ProviderDto Map(Provider p) => new()
        {
            ProviderId = p.ProviderId,
            Name = p.Name,
            Specialty = p.Specialty,
            Credentials = p.Credentials,
            ContactInfo = p.ContactInfo,
            Status = p.Status
        };

        private static string BuildGeneratedProviderEmail(string name)
        {
            var normalized = new string(
                (name ?? "provider")
                    .ToLowerInvariant()
                    .Select(ch => char.IsLetterOrDigit(ch) ? ch : '.')
                    .ToArray()
            ).Trim('.');

            if (string.IsNullOrWhiteSpace(normalized))
            {
                normalized = "provider";
            }

            return $"{normalized}.{TimeZoneHelper.NowIst().Ticks}@careschedule.local";
        }
    }
}
