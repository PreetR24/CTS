using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IWaitlistRepository
    {
        void Add(Waitlist entity);
        void Update(Waitlist entity);
        void Delete(Waitlist entity);
        Waitlist? GetById(int waitId);
        IEnumerable<Waitlist> Search(int? siteId, int? providerId, int? serviceId, int? patientId, string? status);
    }
}
