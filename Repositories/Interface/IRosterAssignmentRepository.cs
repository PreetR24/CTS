using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IRosterAssignmentRepository
    {
        void Add(RosterAssignment entity);
        void Update(RosterAssignment entity);
        RosterAssignment? GetById(int assignmentId);
        IEnumerable<RosterAssignment> Search(int? siteId, int? userId, DateOnly? date, string? status);
    }
}
