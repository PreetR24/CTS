using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class WaitlistRepository(CareScheduleContext _db) : IWaitlistRepository
    {
        public void Add(Waitlist entity)
        {
            _db.Waitlists.Add(entity);
            _db.SaveChanges();
        }

        public void Update(Waitlist entity)
        {
            _db.Waitlists.Update(entity);
            _db.SaveChanges();
        }

        public void Delete(Waitlist entity)
        {
            _db.Waitlists.Remove(entity);
            _db.SaveChanges();
        }

        public Waitlist? GetById(int waitId)
        {
            return _db.Waitlists.FirstOrDefault(w => w.WaitId == waitId);
        }

        public IEnumerable<Waitlist> Search(int? siteId, int? providerId, int? serviceId, int? patientId, string? status)
        {
            var q = _db.Waitlists.AsQueryable();
            if (siteId.HasValue) q = q.Where(w => w.SiteId == siteId.Value);
            if (providerId.HasValue) q = q.Where(w => w.ProviderId == providerId.Value);
            if (serviceId.HasValue) q = q.Where(w => w.ServiceId == serviceId.Value);
            if (patientId.HasValue) q = q.Where(w => w.PatientId == patientId.Value);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(w => w.Status == status);
            return q.OrderByDescending(w => w.WaitId).ToList();
        }
    }
}