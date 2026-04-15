using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ILeaveRequestRepository
    {
        void Add(LeaveRequest entity);
        void Update(LeaveRequest entity);
        LeaveRequest? GetById(int leaveId);
        IEnumerable<LeaveRequest> Search(int? userId, string? status);
    }
}
