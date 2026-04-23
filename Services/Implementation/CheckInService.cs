using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class CheckInService(
            ICheckInRepository _checkInRepo,
            IAppointmentRepository _apptRepo,
            IResourceHoldRepository _resourceHoldRepo,
            IAuditLogService _auditService) : ICheckInService
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
                TokenNo = string.IsNullOrWhiteSpace(dto.TokenNo) ? null : dto.TokenNo.Trim(),
                CheckInTime = DateTime.UtcNow,
                Status = "Waiting"
            };
            _checkInRepo.Add(entity);
            if (string.IsNullOrWhiteSpace(entity.TokenNo))
            {
                entity.TokenNo = GenerateToken(entity);
                _checkInRepo.Update(entity);
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CheckIn",
                Resource = "CheckIn",
                Metadata = $"AppointmentId={appointmentId}; CheckInId={entity.CheckInId}"
            });

            return Map(entity);
        }

        private static string GenerateToken(CheckIn checkIn)
        {
            // Stable, human-readable token generated after DB identity is available.
            // Example: TK-20260423-0012
            return $"TK-{checkIn.CheckInTime:yyyyMMdd}-{checkIn.CheckInId:D4}";
        }

        public CheckInResponseDto AssignRoom(int checkInId, AssignRoomRequestDto dto)
        {
            var entity = GetOrThrow(checkInId);
            if (dto.RoomId <= 0) throw new ArgumentException("RoomId is required.");
            var appt = _apptRepo.GetById(entity.AppointmentId);
            if (appt == null) throw new KeyNotFoundException($"Appointment {entity.AppointmentId} not found.");

            var roomBusy = HasAppointmentRoomConflict(
                dto.RoomId,
                appt.SiteId,
                appt.SlotDate,
                appt.StartTime,
                appt.EndTime,
                appt.AppointmentId);
            if (roomBusy)
                throw new ArgumentException("Selected room is already assigned for this date/time slot.");

            var holdConflict = HasActiveRoomHoldConflict(dto.RoomId, appt.SiteId, appt.SlotDate, appt.StartTime, appt.EndTime);
            if (holdConflict)
                throw new ArgumentException("Selected room is blocked by an active resource hold for this date/time slot.");

            entity.RoomAssigned = dto.RoomId;
            entity.Status = "RoomAssigned";
            _checkInRepo.Update(entity);
            SyncAppointmentStatus(entity.AppointmentId, "RoomAssigned", dto.RoomId);
            CreateRoomHoldForAssignedAppointment(appt, dto.RoomId);

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
            SyncAppointmentStatus(entity.AppointmentId, "InProgress");

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
            var nextStatus = dto.Status.Trim();
            entity.Status = nextStatus;
            _checkInRepo.Update(entity);
            SyncAppointmentStatus(entity.AppointmentId, MapAppointmentStatusFromCheckInStatus(nextStatus));

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
            SyncAppointmentStatus(entity.AppointmentId, "InProgress");

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

        private void SyncAppointmentStatus(int appointmentId, string? appointmentStatus, int? roomId = null)
        {
            if (string.IsNullOrWhiteSpace(appointmentStatus)) return;

            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) return;

            appt.Status = appointmentStatus;
            if (roomId.HasValue && roomId.Value > 0)
                appt.RoomId = roomId.Value;
            _apptRepo.Update(appt);
        }

        private bool HasActiveRoomHoldConflict(int roomId, int siteId, DateOnly slotDate, TimeOnly startTime, TimeOnly endTime)
        {
            var slotStart = new DateTime(slotDate.Year, slotDate.Month, slotDate.Day, startTime.Hour, startTime.Minute, 0);
            var slotEnd = new DateTime(slotDate.Year, slotDate.Month, slotDate.Day, endTime.Hour, endTime.Minute, 0);

            return _resourceHoldRepo
                .Search(siteId, "Room", roomId)
                .Any(h =>
                    string.Equals(h.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                    h.StartTime < slotEnd &&
                    slotStart < h.EndTime);
        }

        private void CreateRoomHoldForAssignedAppointment(Appointment appt, int roomId)
        {
            var start = new DateTime(appt.SlotDate.Year, appt.SlotDate.Month, appt.SlotDate.Day, appt.StartTime.Hour, appt.StartTime.Minute, 0);
            var end = new DateTime(appt.SlotDate.Year, appt.SlotDate.Month, appt.SlotDate.Day, appt.EndTime.Hour, appt.EndTime.Minute, 0);
            var exists = _resourceHoldRepo.Search(appt.SiteId, "Room", roomId).Any(h =>
                string.Equals(h.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                h.StartTime == start &&
                h.EndTime == end);
            if (exists) return;

            _resourceHoldRepo.Add(new ResourceHold
            {
                ResourceType = "Room",
                ResourceId = roomId,
                SiteId = appt.SiteId,
                StartTime = start,
                EndTime = end,
                Reason = $"Appointment #{appt.AppointmentId} room assignment",
                Status = "Active"
            });
        }

        private bool HasAppointmentRoomConflict(
            int roomId,
            int siteId,
            DateOnly slotDate,
            TimeOnly startTime,
            TimeOnly endTime,
            int excludeAppointmentId)
        {
            return _apptRepo
                .Search(patientId: null, providerId: null, siteId: siteId, date: slotDate, status: null)
                .Any(a =>
                    a.AppointmentId != excludeAppointmentId &&
                    a.RoomId == roomId &&
                    !string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(a.Status, "Completed", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(a.Status, "NoShow", StringComparison.OrdinalIgnoreCase) &&
                    a.StartTime < endTime &&
                    startTime < a.EndTime);
        }

        private static string? MapAppointmentStatusFromCheckInStatus(string checkInStatus)
        {
            var status = checkInStatus.Trim();
            if (status.Equals("Waiting", StringComparison.OrdinalIgnoreCase)) return "CheckedIn";
            if (status.Equals("RoomAssigned", StringComparison.OrdinalIgnoreCase)) return "RoomAssigned";
            if (status.Equals("InRoom", StringComparison.OrdinalIgnoreCase)) return "InProgress";
            if (status.Equals("WithProvider", StringComparison.OrdinalIgnoreCase)) return "InProgress";
            if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase)) return "Completed";
            return null;
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