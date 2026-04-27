using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class WaitlistService(
            IWaitlistRepository _waitlistRepo,
            ISiteRepository _siteRepo,
            IProviderRepository _providerRepo,
            IServiceRepository _serviceRepo,
            IUserRepository _userRepo,
            IAuditLogService _auditService) : IWaitlistService
    {
        public WaitlistResponseDto Add(CreateWaitlistRequestDto dto)
        {
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");
            if (dto.ProviderId <= 0) throw new ArgumentException("ProviderId is required.");
            if (dto.ServiceId <= 0) throw new ArgumentException("ServiceId is required.");
            if (dto.PatientId <= 0) throw new ArgumentException("PatientId is required.");
            if (_siteRepo.Get(dto.SiteId) is null) throw new KeyNotFoundException($"Site {dto.SiteId} not found.");
            if (_providerRepo.GetById(dto.ProviderId) is null) throw new KeyNotFoundException($"Provider {dto.ProviderId} not found.");
            if (_serviceRepo.GetById(dto.ServiceId) is null) throw new KeyNotFoundException($"Service {dto.ServiceId} not found.");
            var site = _siteRepo.Get(dto.SiteId)!;
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Site {dto.SiteId} is not active.");
            var provider = _providerRepo.GetById(dto.ProviderId)!;
            if (!string.Equals(provider.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Provider {dto.ProviderId} is not active.");
            var service = _serviceRepo.GetById(dto.ServiceId)!;
            if (!string.Equals(service.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Service {dto.ServiceId} is not active.");
            var patient = _userRepo.GetById(dto.PatientId);
            if (patient is null || !string.Equals(patient.Role, "Patient", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Patient {dto.PatientId} not found.");
            if (!string.Equals(patient.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Patient {dto.PatientId} is not active.");

            DateOnly reqDate = TimeZoneHelper.TodayIst();
            if (!string.IsNullOrWhiteSpace(dto.RequestedDate))
            {
                if (!DateOnly.TryParseExact(dto.RequestedDate.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out reqDate))
                    throw new ArgumentException("Invalid RequestedDate. Use yyyy-MM-dd.");
            }

            var entity = new Waitlist
            {
                SiteId = dto.SiteId,
                ProviderId = dto.ProviderId,
                ServiceId = dto.ServiceId,
                PatientId = dto.PatientId,
                Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Normal" : dto.Priority.Trim(),
                RequestedDate = reqDate,
                Status = "Open"
            };

            _waitlistRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "AddToWaitlist",
                Resource = "Waitlist",
                Metadata = $"WaitId={entity.WaitId}; Patient #{entity.PatientId}"
            });

            return Map(entity);
        }

        public void Remove(int waitId)
        {
            var entity = _waitlistRepo.GetById(waitId);
            if (entity == null) throw new KeyNotFoundException($"Waitlist entry {waitId} not found.");

            _waitlistRepo.Delete(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "RemoveFromWaitlist",
                Resource = "Waitlist",
                Metadata = $"WaitId={waitId} deleted"
            });
        }

        public IEnumerable<WaitlistResponseDto> Search(WaitlistSearchDto dto)
        {
            var items = _waitlistRepo.Search(dto.SiteId, dto.ProviderId, dto.ServiceId, dto.PatientId, dto.Status);
            return items.Select(Map).ToList();
        }

        public WaitlistResponseDto Fill(int waitId, FillWaitlistRequestDto dto)
        {
            var entity = _waitlistRepo.GetById(waitId);
            if (entity == null) throw new KeyNotFoundException($"Waitlist entry {waitId} not found.");
            if (entity.Status is not ("Open" or "Notified"))
                throw new ArgumentException("Only entries with status 'Open' or 'Notified' can be filled.");

            entity.Status = "Filled";
            _waitlistRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "FillWaitlist",
                Resource = "Waitlist",
                Metadata = $"WaitId={waitId} filled"
            });

            return Map(entity);
        }

        private static WaitlistResponseDto Map(Waitlist w) => new()
        {
            WaitId = w.WaitId,
            SiteId = w.SiteId,
            ProviderId = w.ProviderId,
            ServiceId = w.ServiceId,
            PatientId = w.PatientId,
            Priority = w.Priority,
            RequestedDate = w.RequestedDate.ToString("yyyy-MM-dd"),
            Status = w.Status
        };
    }
}