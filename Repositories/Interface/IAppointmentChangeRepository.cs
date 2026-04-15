using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IAppointmentChangeRepository
    {
        void Add(AppointmentChange entity);
    }
}