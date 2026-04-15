using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OutcomeRepository(CareScheduleContext _db) : IOutcomeRepository
    {
        public void Add(Outcome entity)
        {
            _db.Outcomes.Add(entity);
            _db.SaveChanges();
        }

        public Outcome? GetByAppointmentId(int appointmentId)
        {
            return _db.Outcomes.FirstOrDefault(o => o.AppointmentId == appointmentId);
        }
    }
}