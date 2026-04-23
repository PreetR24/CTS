using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class ProviderServiceRepository(CareScheduleContext _db) : IProviderServiceRepository
    {
        public List<ProviderService> GetByProviderId(int providerId)
        {
            return _db.ProviderServices
                .AsNoTracking()
                .Include(ps => ps.Service)
                .Include(ps => ps.Provider)
                .Where(ps => ps.ProviderId == providerId)
                .ToList();
        }

        public List<ProviderService> GetByServiceId(int serviceId)
        {
            return _db.ProviderServices
                .AsNoTracking()
                .Include(ps => ps.Provider)
                .Include(ps => ps.Service)
                .Where(ps => ps.ServiceId == serviceId)
                .ToList();
        }

        public ProviderService? GetByProviderAndService(int providerId, int serviceId)
        {
            return _db.ProviderServices
                .AsNoTracking()
                .FirstOrDefault(ps => ps.ProviderId == providerId && ps.ServiceId == serviceId);
        }

        public ProviderService? GetById(int psid)
        {
            return _db.ProviderServices
                .AsNoTracking()
                .Include(ps => ps.Provider)
                .Include(ps => ps.Service)
                .FirstOrDefault(ps => ps.Psid == psid);
        }

        public ProviderService Create(ProviderService entity)
        {
            _db.ProviderServices.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Delete(ProviderService entity)
        {
            _db.ProviderServices.Remove(entity);
            _db.SaveChanges();
        }

        public IEnumerable<ProviderService> GetActiveByProvider(int providerId)
        {
            return _db.ProviderServices
                      .Where(ps => ps.ProviderId == providerId
                                   && ps.Status != null
                                   && ps.Status.Trim().ToLower() == "active")
                      .ToList();
        }

        public bool AnyActiveByService(int serviceId)
        {
            return _db.ProviderServices.Any(ps => ps.ServiceId == serviceId
                                                  && ps.Status != null
                                                  && ps.Status.Trim().ToLower() == "active");
        }
    }
}
