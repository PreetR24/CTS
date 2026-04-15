using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class ShiftTemplateRepository(CareScheduleContext _db) : IShiftTemplateRepository
    {
        public void Add(ShiftTemplate entity)
        {
            _db.ShiftTemplates.Add(entity);
            _db.SaveChanges();
        }

        public void Update(ShiftTemplate entity)
        {
            _db.ShiftTemplates.Update(entity);
            _db.SaveChanges();
        }

        public ShiftTemplate? GetById(int shiftTemplateId)
        {
            return _db.ShiftTemplates
                .AsNoTracking()
                .FirstOrDefault(s => s.ShiftTemplateId == shiftTemplateId);
        }

        public IEnumerable<ShiftTemplate> Search(int? siteId, string? role, string? status)
        {
            var q = _db.ShiftTemplates.AsNoTracking().AsQueryable();

            if (siteId.HasValue)
            {
                var id = siteId.Value;
                q = q.Where(s => s.SiteId == id);
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var r = role.Trim();
                q = q.Where(s => EF.Functions.Like(s.Role.ToLower(), $"%{r.ToLower()}%"));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim();
                q = q.Where(s => s.Status == st);
            }

            return q.ToList();
        }
    }
}
