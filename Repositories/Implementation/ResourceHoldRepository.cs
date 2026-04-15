using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ResourceHoldRepository(CareScheduleContext _db) : IResourceHoldRepository
    {
        public void Add(ResourceHold entity)
        {
            _db.ResourceHolds.Add(entity);
            _db.SaveChanges();
        }

        public void Update(ResourceHold entity)
        {
            _db.ResourceHolds.Update(entity);
            _db.SaveChanges();
        }

        public ResourceHold? GetById(int holdId)
        {
            return _db.ResourceHolds.FirstOrDefault(r => r.HoldId == holdId);
        }

        public IEnumerable<ResourceHold> Search(int? siteId, string? resourceType, int? resourceId)
        {
            var query = _db.ResourceHolds.AsQueryable();

            if (siteId.HasValue)
                query = query.Where(r => r.SiteId == siteId.Value);
            if (!string.IsNullOrWhiteSpace(resourceType))
                query = query.Where(r => r.ResourceType == resourceType);
            if (resourceId.HasValue)
                query = query.Where(r => r.ResourceId == resourceId.Value);

            return query.OrderByDescending(r => r.StartTime).ToList();
        }
    }
}