using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class SlaRepository(CareScheduleContext _db) : ISlaRepository
    {
        public void Add(Sla entity)
        {
            _db.Slas.Add(entity);
            _db.SaveChanges();
        }

        public void Update(Sla entity)
        {
            _db.Slas.Update(entity);
            _db.SaveChanges();
        }

        public Sla? GetById(int slaId)
        {
            return _db.Slas.FirstOrDefault(s => s.Slaid == slaId);
        }

        public IEnumerable<Sla> Search(string? scope, string? status)
        {
            var q = _db.Slas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(scope))
                q = q.Where(s => s.Scope == scope);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(s => s.Status == status);

            return q.OrderBy(s => s.Metric).ToList();
        }
    }
}