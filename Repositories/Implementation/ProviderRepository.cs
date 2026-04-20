using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class ProviderRepository(CareScheduleContext _db) : IProviderRepository
    {
        public List<Provider> GetAll()
        {
            return _db.Providers.AsNoTracking().ToList();
        }

        public Provider? GetById(int id)
        {
            return _db.Providers.AsNoTracking().FirstOrDefault(p => p.ProviderId == id);
        }

        public Provider Create(Provider entity)
        {
            _db.Providers.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public Provider CreateWithId(Provider entity, int providerId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid providerId.");
            entity.ProviderId = providerId;
            _db.Providers.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Provider entity)
        {
            _db.Providers.Update(entity);
            _db.SaveChanges();
        }
    }
}
