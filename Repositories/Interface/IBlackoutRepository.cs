using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IBlackoutRepository
    {
        void Add(Blackout entity);
        void Update(Blackout entity);
        Blackout? GetById(int blackoutId);
        IEnumerable<Blackout> ListBySite(int siteId);
        IEnumerable<Blackout> ListBySiteDateRange(int siteId, DateOnly startDate, DateOnly endDate);
    }
}
