using System.Collections.Generic;
using CareSchedule.Models; // adjust to your actual models namespace

namespace CareSchedule.Repositories.Interface
{
    public interface IAvailabilityTemplateRepository
    {
        void Add(AvailabilityTemplate entity);
        void Update(AvailabilityTemplate entity);
        AvailabilityTemplate? GetById(int templateId);
        IEnumerable<AvailabilityTemplate> List(int providerId, int siteId);
        IEnumerable<AvailabilityTemplate> ListBySiteActive(int siteId);
        bool AnyActiveByProvider(int providerId);
    }
}