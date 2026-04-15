using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class AppointmentChangeRepository(CareScheduleContext _db) : IAppointmentChangeRepository
    {
        public void Add(AppointmentChange entity) => _db.AppointmentChanges.Add(entity);
    }
}