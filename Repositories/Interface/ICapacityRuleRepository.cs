using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ICapacityRuleRepository
    {
        void Add(CapacityRule entity);
        void Update(CapacityRule entity);
        CapacityRule? GetById(int ruleId);
        IEnumerable<CapacityRule> Search(string? scope, string? status);
        IEnumerable<CapacityRule> GetActiveByDate(DateOnly date);
    }
}
