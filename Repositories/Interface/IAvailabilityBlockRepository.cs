using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IAvailabilityBlockRepository
    {
        void Add(AvailabilityBlock entity);
        void Update(AvailabilityBlock entity);   
        AvailabilityBlock? GetById(int blockId);
        IEnumerable<AvailabilityBlock> List(int providerId, int siteId, DateOnly? date);
    }
}