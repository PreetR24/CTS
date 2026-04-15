using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class LeaveRequestRepository(CareScheduleContext _db) : ILeaveRequestRepository
    {
        public void Add(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            _db.SaveChanges();
        }

        public void Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            _db.SaveChanges();
        }

        public LeaveRequest? GetById(int leaveId)
        {
            return _db.LeaveRequests.FirstOrDefault(l => l.LeaveId == leaveId);
        }

        public IEnumerable<LeaveRequest> Search(int? userId, string? status)
        {
            var q = _db.LeaveRequests.AsQueryable();

            if (userId.HasValue)
                q = q.Where(l => l.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(l => l.Status == status);

            return q.OrderByDescending(l => l.SubmittedDate).ToList();
        }
    }
}
