using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ReminderScheduleRepository(CareScheduleContext _db) : IReminderScheduleRepository
    {
        public void Add(ReminderSchedule entity) => _db.ReminderSchedules.Add(entity);

        public IEnumerable<ReminderSchedule> GetPendingByAppointmentId(int appointmentId)
        {
            return _db.ReminderSchedules
                .Where(r => r.AppointmentId == appointmentId && r.Status == "Pending")
                .ToList();
        }

        public IEnumerable<ReminderSchedule> GetDueReminders(DateTime cutoffUtc)
        {
            return _db.ReminderSchedules
                .Include(r => r.Appointment)
                .Where(r => r.Status == "Pending")
                .AsEnumerable()
                .Where(r =>
                {
                    var appt = r.Appointment;
                    var fireTime = appt.SlotDate.ToDateTime(appt.StartTime)
                                       .AddMinutes(-r.RemindOffsetMin);
                    return fireTime <= cutoffUtc;
                })
                .ToList();
        }

        public void Update(ReminderSchedule entity) => _db.ReminderSchedules.Update(entity);
    }
}