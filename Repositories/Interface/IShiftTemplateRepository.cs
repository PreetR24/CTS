using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IShiftTemplateRepository
    {
        void Add(ShiftTemplate entity);
        void Update(ShiftTemplate entity);
        ShiftTemplate? GetById(int shiftTemplateId);
        IEnumerable<ShiftTemplate> Search(int? siteId, string? role, string? status);
    }
}
