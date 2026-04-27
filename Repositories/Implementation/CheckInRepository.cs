using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class CheckInRepository(CareScheduleContext _db) : ICheckInRepository
    {
        public void Add(CheckIn entity)
        {
            _db.CheckIns.Add(entity);
            _db.SaveChanges();
        }

        public void Update(CheckIn entity)
        {
            _db.CheckIns.Update(entity);
            _db.SaveChanges();
        }

        public CheckIn? GetById(int checkInId)
        {
            return _db.CheckIns
                .Include(c => c.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefault(c => c.CheckInId == checkInId);
        }

        public CheckIn? GetByAppointmentId(int appointmentId)
        {
            return _db.CheckIns
                .Include(c => c.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefault(c => c.AppointmentId == appointmentId);
        }

        public IEnumerable<CheckIn> Search(int? siteId, int? providerId, int? nurseId, string? status)
        {
            var q = _db.CheckIns
                .Include(c => c.Appointment)
                .ThenInclude(a => a.Patient)
                .AsQueryable();
            if (siteId.HasValue) q = q.Where(c => c.Appointment.SiteId == siteId.Value);
            if (providerId.HasValue) q = q.Where(c => c.Appointment.ProviderId == providerId.Value);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(c => c.Status == status);
            return q.OrderByDescending(c => c.CheckInTime).ToList();
        }
    }
}