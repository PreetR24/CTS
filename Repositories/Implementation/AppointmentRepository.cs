using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class AppointmentRepository(CareScheduleContext _db) : IAppointmentRepository
    {
        public void Add(Appointment entity) => _db.Appointments.Add(entity);
        public void Update(Appointment entity) => _db.Appointments.Update(entity);
        public void AddChange(AppointmentChange entity) => _db.AppointmentChanges.Add(entity);

        public Appointment? GetById(int appointmentId)
        {
            return _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Provider)
                .Include(a => a.Service)
                .Include(a => a.Site)
                .Include(a => a.Room)
                .FirstOrDefault(a => a.AppointmentId == appointmentId);
        }

        public IEnumerable<Appointment> Search(int? patientId, int? providerId, int? siteId, DateOnly? date, string? status)
        {
            var q = _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Provider)
                .Include(a => a.Service)
                .Include(a => a.Site)
                .Include(a => a.Room)
                .AsQueryable();

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

        public int CountReschedulesForAppointmentOnDay(int appointmentId, DateOnly day)
        {
            var start = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
            var end = start.AddDays(1);
            return _db.AppointmentChanges.Count(c =>
                c.AppointmentId == appointmentId &&
                c.ChangeType == "Reschedule" &&
                c.ChangedDate >= start &&
                c.ChangedDate < end);
        }

        public string? GetLatestRescheduleReason(int appointmentId)
        {
            return _db.AppointmentChanges
                .Where(c => c.AppointmentId == appointmentId && c.ChangeType == "Reschedule")
                .OrderByDescending(c => c.ChangedDate)
                .Select(c => c.Reason)
                .FirstOrDefault();
        }
    }
}