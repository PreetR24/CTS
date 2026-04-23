using System;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class BillingService(
            IChargeRefRepository _chargeRepo,
            IAuditLogService _auditService,
            CareScheduleContext _db)
            : IBillingService
    {
        public ChargeRefResponseDto CreateCharge(CreateChargeRefDto dto)
        {
            if (dto.AppointmentId <= 0) throw new ArgumentException("AppointmentId is required.");
            if (dto.ServiceId <= 0) throw new ArgumentException("ServiceId is required.");
            if (dto.ProviderId <= 0) throw new ArgumentException("ProviderId is required.");
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be positive.");

            var entity = new ChargeRef
            {
                AppointmentId = dto.AppointmentId,
                ServiceId = dto.ServiceId,
                ProviderId = dto.ProviderId,
                Amount = dto.Amount,
                Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "INR" : dto.Currency.Trim(),
                Status = "Open"
            };

            _chargeRepo.Add(entity);
            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateCharge",
                Resource = "ChargeRef",
                Metadata = $"{{\"appointmentId\":{dto.AppointmentId}}}"
            });
            _db.SaveChanges();
            return Map(entity);
        }

        public ChargeRefResponseDto GetByAppointment(int appointmentId)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid appointmentId.");
            var entity = _chargeRepo.GetByAppointmentId(appointmentId);
            if (entity == null) throw new KeyNotFoundException("Charge not found.");
            return Map(entity);
        }

        public IEnumerable<ChargeRefResponseDto> Search(ChargeSearchDto dto)
            => _chargeRepo.Search(dto.AppointmentId, dto.ProviderId, dto.Status).Select(Map).ToList();

        private static ChargeRefResponseDto Map(ChargeRef c) => new()
        {
            ChargeRefId = c.ChargeRefId,
            AppointmentId = c.AppointmentId,
            ServiceId = c.ServiceId,
            ProviderId = c.ProviderId,
            Amount = c.Amount,
            Currency = c.Currency,
            Status = c.Status
        };
    }
}