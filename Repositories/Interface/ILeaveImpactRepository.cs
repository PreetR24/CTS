using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ILeaveImpactRepository
    {
        void Add(LeaveImpact entity);
        void Update(LeaveImpact entity);
        LeaveImpact? GetById(int impactId);
        IEnumerable<LeaveImpact> GetByLeaveId(int leaveId);
    }
}
