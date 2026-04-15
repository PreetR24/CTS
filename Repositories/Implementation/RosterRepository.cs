using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class RosterRepository(CareScheduleContext _db) : IRosterRepository
    {
        public void Add(Roster entity)
        {
            _db.Rosters.Add(entity);
            _db.SaveChanges();
        }

        public void Update(Roster entity)
        {
            _db.Rosters.Update(entity);
            _db.SaveChanges();
        }

        public Roster? GetById(int rosterId)
        {
            return _db.Rosters
                .AsNoTracking()
                .FirstOrDefault(r => r.RosterId == rosterId);
        }

        public IEnumerable<Roster> Search(int? siteId, string? status)
        {
            var q = _db.Rosters.AsNoTracking().AsQueryable();

            if (siteId.HasValue)
            {
                var id = siteId.Value;
                q = q.Where(r => r.SiteId == id);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim();
                q = q.Where(r => r.Status == st);
            }

            return q.ToList();
        }
    }
}
