using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IOutcomeRepository
    {
        void Add(Outcome entity);
        Outcome? GetByAppointmentId(int appointmentId);
    }
}
