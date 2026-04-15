using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IReminderScheduleRepository
    {
        void Add(ReminderSchedule entity);
        IEnumerable<ReminderSchedule> GetPendingByAppointmentId(int appointmentId);
        IEnumerable<ReminderSchedule> GetDueReminders(DateTime cutoffUtc);
        void Update(ReminderSchedule entity);
    }
}