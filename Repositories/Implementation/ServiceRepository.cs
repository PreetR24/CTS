using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class ServiceRepository(CareScheduleContext _db) : IServiceRepository
    {
        public List<Service> GetAll()
        {
            return _db.Services.AsNoTracking().ToList();
        }

        public Service? GetById(int id)
        {
            return _db.Services.AsNoTracking().FirstOrDefault(s => s.ServiceId == id);
        }

        public Service Create(Service entity)
        {
            _db.Services.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Service entity)
        {
            _db.Services.Update(entity);
            _db.SaveChanges();
        }
    }
}
