using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class CheckInService(
            ICheckInRepository _checkInRepo,
            IAppointmentRepository _apptRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            IUnitOfWork _uow) : ICheckInService
    {
        public CheckInResponseDto CheckIn(int appointmentId, CreateCheckInRequestDto dto)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid appointmentId.");

            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException($"Appointment {appointmentId} not found.");
            if (appt.Status != "Booked")
                throw new ArgumentException("Only 'Booked' appointments can be checked in.");

            var existing = _checkInRepo.GetByAppointmentId(appointmentId);
            if (existing != null) throw new ArgumentException("Patient already checked in for this appointment.");

            appt.Status = "CheckedIn";
            _apptRepo.Update(appt);

            var entity = new CheckIn
            {
                AppointmentId = appointmentId,
                TokenNo = dto.TokenNo,
                CheckInTime = DateTime.UtcNow,
                Status = "Waiting"
            };
            _checkInRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CheckIn",
                Resource = "CheckIn",
                Metadata = $"AppointmentId={appointmentId}; CheckInId={entity.CheckInId}"
            });

            return Map(entity);
        }

        public CheckInResponseDto AssignRoom(int checkInId, AssignRoomRequestDto dto)
        {
            var entity = GetOrThrow(checkInId);
            if (dto.RoomId <= 0) throw new ArgumentException("RoomId is required.");

            entity.RoomAssigned = dto.RoomId;
            entity.Status = "RoomAssigned";
            _checkInRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "AssignRoom",
                Resource = "CheckIn",
                Metadata = $"CheckInId={checkInId}; RoomId={dto.RoomId}"
            });

            return Map(entity);
        }

        public CheckInResponseDto MoveToRoom(int checkInId)
        {
            var entity = GetOrThrow(checkInId);
            if (entity.RoomAssigned == null)
                throw new ArgumentException("No room assigned yet.");

            entity.Status = "InRoom";
            _checkInRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "MoveToRoom",
                Resource = "CheckIn",
                Metadata = $"CheckInId={checkInId}"
            });

            return Map(entity);
        }

        public CheckInResponseDto UpdateStatus(int checkInId, UpdateCheckInStatusDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status is required.");

            var entity = GetOrThrow(checkInId);
            entity.Status = dto.Status.Trim();
            _checkInRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateCheckInStatus",
                Resource = "CheckIn",
                Metadata = $"CheckInId={checkInId}; NewStatus={dto.Status.Trim()}"
            });

            return Map(entity);
        }

        public CheckInResponseDto StartConsultation(int checkInId)
        {
            var entity = GetOrThrow(checkInId);
            entity.Status = "WithProvider";
            _checkInRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "StartConsultation",
                Resource = "CheckIn",
                Metadata = $"CheckInId={checkInId}"
            });

            return Map(entity);
        }

        public IEnumerable<CheckInResponseDto> Search(CheckInSearchDto dto)
        {
            var items = _checkInRepo.Search(dto.SiteId, dto.ProviderId, dto.NurseId, dto.Status);
            return items.Select(Map).ToList();
        }

        public CheckInResponseDto GetById(int checkInId)
        {
            return Map(GetOrThrow(checkInId));
        }

        private CheckIn GetOrThrow(int checkInId)
        {
            var entity = _checkInRepo.GetById(checkInId);
            if (entity == null) throw new KeyNotFoundException($"CheckIn {checkInId} not found.");
            return entity;
        }

        private static CheckInResponseDto Map(CheckIn c) => new()
        {
            CheckInId = c.CheckInId,
            AppointmentId = c.AppointmentId,
            TokenNo = c.TokenNo,
            CheckInTime = c.CheckInTime,
            RoomAssigned = c.RoomAssigned,
            Status = c.Status
        };
    }
}