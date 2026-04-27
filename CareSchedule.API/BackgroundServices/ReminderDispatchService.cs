using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.API.BackgroundServices
{
    public class ReminderDispatchService(
        IServiceScopeFactory _scopeFactory,
        ILogger<ReminderDispatchService> _logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderDispatchService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    DispatchDueReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error dispatching reminders.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private void DispatchDueReminders()
        {
            using var scope = _scopeFactory.CreateScope();
            var reminderRepo = scope.ServiceProvider.GetRequiredService<IReminderScheduleRepository>();
            var notifRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var db = scope.ServiceProvider.GetRequiredService<CareScheduleContext>();

            var now = TimeZoneHelper.NowIst();
            var dueReminders = reminderRepo.GetDueReminders(now);

            var count = 0;
            foreach (var reminder in dueReminders)
            {
                var appt = reminder.Appointment;
                if (appt == null) continue;

                notifRepo.Add(new Notification
                {
                    UserId = appt.PatientId,
                    Message = $"Reminder: You have an appointment on {appt.SlotDate:yyyy-MM-dd} at {appt.StartTime:HH\\:mm}.",
                    Category = "Appointment",
                    Status = "Unread",
                    CreatedDate = TimeZoneHelper.NowIst()
                });

                reminder.Status = "Sent";
                reminderRepo.Update(reminder);
                count++;
            }

            if (count > 0)
            {
                db.SaveChanges();
                _logger.LogInformation("Dispatched {Count} reminder(s).", count);
            }
        }
    }
}
