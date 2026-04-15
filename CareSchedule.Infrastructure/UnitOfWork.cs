using CareSchedule.Infrastructure.Data;

namespace CareSchedule.Infrastructure
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }

    public class UnitOfWork(CareScheduleContext _db) : IUnitOfWork
    {
        public int SaveChanges()
        {
            return _db.SaveChanges();
        }
    }
}