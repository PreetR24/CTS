using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OpsReportRepository(CareScheduleContext _db) : IOpsReportRepository
    {
        public void Add(OpsReport entity)
        {
            _db.OpsReports.Add(entity);
        }

        public OpsReport? GetById(int reportId)
        {
            return _db.OpsReports.FirstOrDefault(r => r.ReportId == reportId);
        }

        public IEnumerable<OpsReport> Search(string? scope, DateTime? from, DateTime? to)
        {
            var q = _db.OpsReports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(scope))
                q = q.Where(r => r.Scope == scope.Trim());
            if (from.HasValue)
                q = q.Where(r => r.GeneratedDate >= from.Value);
            if (to.HasValue)
                q = q.Where(r => r.GeneratedDate <= to.Value);

            return q.OrderByDescending(r => r.GeneratedDate).ToList();
        }
    }
}
