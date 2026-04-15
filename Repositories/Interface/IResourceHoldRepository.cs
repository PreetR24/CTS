using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IResourceHoldRepository
    {
        void Add(ResourceHold entity);
        void Update(ResourceHold entity);
        ResourceHold? GetById(int holdId);
        IEnumerable<ResourceHold> Search(int? siteId, string? resourceType, int? resourceId);
    }
}
