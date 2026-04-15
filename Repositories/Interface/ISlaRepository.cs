using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ISlaRepository
    {
        void Add(Sla entity);
        void Update(Sla entity);
        Sla? GetById(int slaId);
        IEnumerable<Sla> Search(string? scope, string? status);
    }
}
