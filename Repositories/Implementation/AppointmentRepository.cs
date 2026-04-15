using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class AppointmentRepository(CareScheduleContext _db) : IAppointmentRepository
    {
        public void Add(Appointment entity) => _db.Appointments.Add(entity);
        public void Update(Appointment entity) => _db.Appointments.Update(entity);

        public Appointment? GetById(int appointmentId)
        {
            return _db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
        }

        public IEnumerable<Appointment> Search(int? patientId, int? providerId, int? siteId, DateOnly? date, string? status)
        {
            var q = _db.Appointments.AsQueryable();

            if (patientId.HasValue) q = q.Where(a => a.PatientId == patientId.Value);
            if (providerId.HasValue) q = q.Where(a => a.ProviderId == providerId.Value);
            if (siteId.HasValue) q = q.Where(a => a.SiteId == siteId.Value);
            if (date.HasValue) q = q.Where(a => a.SlotDate == date.Value);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(a => a.Status == status);

            return q.OrderBy(a => a.SlotDate).ThenBy(a => a.StartTime).ToList();
        }

        public int CountByProviderDate(int providerId, int siteId, DateOnly date, string status)
        {
            return _db.Appointments.Count(a =>
                a.ProviderId == providerId &&
                a.SiteId == siteId &&
                a.SlotDate == date &&
                a.Status == status);
        }
    }
}