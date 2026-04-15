using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class AvailabilityBlockRepository(CareScheduleContext _db) : IAvailabilityBlockRepository
    {
        public void Add(AvailabilityBlock entity)
        {
            _db.AvailabilityBlocks.Add(entity);
        }

        public void Update(AvailabilityBlock entity)
        {
            _db.AvailabilityBlocks.Update(entity);
        }

        public AvailabilityBlock? GetById(int blockId)
        {
            return _db.AvailabilityBlocks.FirstOrDefault(b => b.BlockId == blockId);
        }

        public IEnumerable<AvailabilityBlock> List(int providerId, int siteId, DateOnly? date)
        {
            var q = _db.AvailabilityBlocks.AsQueryable()
                     .Where(b => b.ProviderId == providerId && b.SiteId == siteId);

            if (date.HasValue)
            {
                var d = date.Value;
                q = q.Where(b => b.Date == d);
            }

            return q.OrderBy(b => b.Date)
                    .ThenBy(b => b.StartTime)
                    .ToList();
        }
    }
}