using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IOpsReportRepository
    {
        void Add(OpsReport entity);
        OpsReport? GetById(int reportId);
        IEnumerable<OpsReport> Search(string? scope, DateTime? from, DateTime? to);
    }
}
