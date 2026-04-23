using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class BlackoutRepository(CareScheduleContext _db) : IBlackoutRepository
    {
        public void Add(Blackout entity) => _db.Blackouts.Add(entity);

        public void Update(Blackout entity) => _db.Blackouts.Update(entity);

        public Blackout? GetById(int blackoutId)
        {
            return _db.Blackouts.FirstOrDefault(b => b.BlackoutId == blackoutId);
        }

        public IEnumerable<Blackout> ListBySite(int siteId)
        {
            return _db.Blackouts
                .Where(b => b.SiteId == siteId)
                .OrderBy(b => b.StartDate)
                .ToList();
        }

        public IEnumerable<Blackout> ListBySiteDateRange(int siteId, DateOnly startDate, DateOnly endDate)
        {
            return _db.Blackouts
                .Where(b => b.SiteId == siteId
                    && b.StartDate <= endDate
                    && b.EndDate >= startDate)
                .OrderBy(b => b.StartDate)
                .ToList();
        }
    }
}
