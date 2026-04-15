using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class RosterAssignmentRepository(CareScheduleContext _db) : IRosterAssignmentRepository
    {
        public void Add(RosterAssignment entity)
        {
            _db.RosterAssignments.Add(entity);
            _db.SaveChanges();
        }

        public void Update(RosterAssignment entity)
        {
            _db.RosterAssignments.Update(entity);
            _db.SaveChanges();
        }

        public RosterAssignment? GetById(int assignmentId)
        {
            return _db.RosterAssignments
                .AsNoTracking()
                .FirstOrDefault(a => a.AssignmentId == assignmentId);
        }

        public IEnumerable<RosterAssignment> Search(int? siteId, int? userId, DateOnly? date, string? status)
        {
            var q = _db.RosterAssignments.AsNoTracking().AsQueryable();

            if (siteId.HasValue)
            {
                var id = siteId.Value;
                q = q.Where(a => a.Roster.SiteId == id);
            }

            if (userId.HasValue)
            {
                var id = userId.Value;
                q = q.Where(a => a.UserId == id);
            }

            if (date.HasValue)
            {
                var d = date.Value;
                q = q.Where(a => a.Date == d);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim();
                q = q.Where(a => a.Status == st);
            }

            return q.ToList();
        }
    }
}
