using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class AvailabilityTemplateRepository(CareScheduleContext _db) : IAvailabilityTemplateRepository
    {
        public void Add(AvailabilityTemplate entity)
        {
            _db.AvailabilityTemplates.Add(entity);
        }

        public void Update(AvailabilityTemplate entity)
        {
            _db.AvailabilityTemplates.Update(entity);
        }

        public AvailabilityTemplate? GetById(int templateId)
        {
            return _db.AvailabilityTemplates.FirstOrDefault(t => t.TemplateId == templateId);
        }

        public IEnumerable<AvailabilityTemplate> List(int providerId, int siteId)
        {
            return _db.AvailabilityTemplates
                .Where(t => t.ProviderId == providerId && t.SiteId == siteId)
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .ToList();
        }

        public IEnumerable<AvailabilityTemplate> ListBySiteActive(int siteId)
        {
            return _db.AvailabilityTemplates
                      .Where(t => t.SiteId == siteId && t.Status == "Active")
                      .OrderBy(t => t.ProviderId)
                      .ThenBy(t => t.DayOfWeek)
                      .ThenBy(t => t.StartTime)
                      .ToList();
        }

        public bool AnyActiveByProvider(int providerId)
        {
            return _db.AvailabilityTemplates.Any(t => t.ProviderId == providerId && t.Status == "Active");
        }
    }
}