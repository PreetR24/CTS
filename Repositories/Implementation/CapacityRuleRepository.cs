using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class CapacityRuleRepository(CareScheduleContext _db) : ICapacityRuleRepository
    {
        public void Add(CapacityRule entity)
        {
            _db.CapacityRules.Add(entity);
            _db.SaveChanges();
        }

        public void Update(CapacityRule entity)
        {
            _db.CapacityRules.Update(entity);
            _db.SaveChanges();
        }

        public CapacityRule? GetById(int ruleId)
        {
            return _db.CapacityRules.FirstOrDefault(r => r.RuleId == ruleId);
        }

        public IEnumerable<CapacityRule> Search(string? scope, string? status)
        {
            var q = _db.CapacityRules.AsQueryable();

            if (!string.IsNullOrWhiteSpace(scope))
                q = q.Where(r => r.Scope == scope);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(r => r.Status == status);

            return q.OrderByDescending(r => r.EffectiveFrom).ToList();
        }

        public IEnumerable<CapacityRule> GetActiveByDate(DateOnly date)
        {
            return _db.CapacityRules
                .Where(r => r.Status == "Active"
                    && r.EffectiveFrom <= date
                    && (r.EffectiveTo == null || r.EffectiveTo >= date))
                .ToList();
        }
    }
}