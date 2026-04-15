using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class LeaveImpactRepository(CareScheduleContext _db) : ILeaveImpactRepository
    {
        public void Add(LeaveImpact entity)
        {
            _db.LeaveImpacts.Add(entity);
            _db.SaveChanges();
        }

        public void Update(LeaveImpact entity)
        {
            _db.LeaveImpacts.Update(entity);
            _db.SaveChanges();
        }

        public LeaveImpact? GetById(int impactId)
        {
            return _db.LeaveImpacts.FirstOrDefault(i => i.ImpactId == impactId);
        }

        public IEnumerable<LeaveImpact> GetByLeaveId(int leaveId)
        {
            return _db.LeaveImpacts
                .Where(i => i.LeaveId == leaveId)
                .ToList();
        }
    }
}
