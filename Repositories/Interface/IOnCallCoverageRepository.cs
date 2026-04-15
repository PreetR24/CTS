using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IOnCallCoverageRepository
    {
        void Add(OnCallCoverage entity);
        void Update(OnCallCoverage entity);
        OnCallCoverage? GetById(int onCallId);
        IEnumerable<OnCallCoverage> Search(int? siteId, DateOnly? date);
    }
}
