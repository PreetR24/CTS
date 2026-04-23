using System;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class OutcomeService(
            IOutcomeRepository _outcomeRepo,
            IAppointmentRepository _apptRepo,
            IPublishedSlotBookingRepository _slotRepo,
            IChargeRefRepository _chargeRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            CareScheduleContext _db) : IOutcomeService
    {
        public OutcomeResponseDto? GetOutcomeByAppointment(int appointmentId)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid appointmentId.");
            var outcome = _outcomeRepo.GetByAppointmentId(appointmentId);
            return outcome == null ? null : Map(outcome);
        }

        public OutcomeResponseDto RecordOutcome(int appointmentId, RecordOutcomeRequestDto dto)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid appointmentId.");
            if (string.IsNullOrWhiteSpace(dto.Outcome)) throw new ArgumentException("Outcome is required.");

            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException($"Appointment {appointmentId} not found.");
            if (!string.Equals(appt.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Outcome can be recorded only after appointment is marked Completed.");

            var existing = _outcomeRepo.GetByAppointmentId(appointmentId);
            if (existing != null) throw new ArgumentException("Outcome already recorded for this appointment.");

            var entity = new Outcome
            {
                AppointmentId = appointmentId,
                Outcome1 = dto.Outcome.Trim(),
                Notes = dto.Notes,
                MarkedBy = dto.MarkedBy,
                MarkedDate = DateTime.UtcNow
            };
            _outcomeRepo.Add(entity);

            var slot = _slotRepo.FindExact(appt.ProviderId, appt.SiteId, appt.SlotDate, appt.StartTime, appt.EndTime);
            if (slot != null && slot.Status != "Closed")
            {
                slot.Status = "Closed";
                _slotRepo.Update(slot);
            }

            _chargeRepo.Add(new ChargeRef
            {
                AppointmentId = appointmentId,
                ServiceId = appt.ServiceId,
                ProviderId = appt.ProviderId,
                Amount = 0,
                Currency = "USD",
                Status = "Open"
            });

            _notifRepo.Add(new Notification
            {
                UserId = appt.PatientId,
                Message = $"Your appointment on {appt.SlotDate:yyyy-MM-dd} has been completed.",
                Category = "Outcome",
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            });
            _db.SaveChanges();

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "RecordOutcome",
                Resource = "Outcome",
                Metadata = $"AppointmentId={appointmentId}; Outcome={dto.Outcome.Trim()}"
            });

            return Map(entity);
        }

        private static OutcomeResponseDto Map(Outcome o) => new()
        {
            OutcomeId = o.OutcomeId,
            AppointmentId = o.AppointmentId,
            Outcome = o.Outcome1,
            Notes = o.Notes,
            MarkedBy = o.MarkedBy,
            MarkedDate = o.MarkedDate
        };
    }
}