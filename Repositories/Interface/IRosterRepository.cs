using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IRosterRepository
    {
        void Add(Roster entity);
        void Update(Roster entity);
        Roster? GetById(int rosterId);
        IEnumerable<Roster> Search(int? siteId, string? status);
    }
}
