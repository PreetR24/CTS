using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class OnCallCoverageRepository(CareScheduleContext _db) : IOnCallCoverageRepository
    {
        public void Add(OnCallCoverage entity)
        {
            _db.OnCallCoverages.Add(entity);
            _db.SaveChanges();
        }

        public void Update(OnCallCoverage entity)
        {
            _db.OnCallCoverages.Update(entity);
            _db.SaveChanges();
        }

        public OnCallCoverage? GetById(int onCallId)
        {
            return _db.OnCallCoverages
                .AsNoTracking()
                .FirstOrDefault(o => o.OnCallId == onCallId);
        }

        public IEnumerable<OnCallCoverage> Search(int? siteId, DateOnly? date)
        {
            var q = _db.OnCallCoverages.AsNoTracking().AsQueryable();

            if (siteId.HasValue)
            {
                var id = siteId.Value;
                q = q.Where(o => o.SiteId == id);
            }

            if (date.HasValue)
            {
                var d = date.Value;
                q = q.Where(o => o.Date == d);
            }

            return q.ToList();
        }
    }
}
