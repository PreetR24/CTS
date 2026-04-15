using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class NotificationRepository(CareScheduleContext _db) : INotificationRepository
    {
        public void Add(Notification entity) => _db.Notifications.Add(entity);

        public Notification? GetById(int notificationId)
        {
            return _db.Notifications.FirstOrDefault(n => n.NotificationId == notificationId);
        }

        public IEnumerable<Notification> GetByUserId(int userId)
        {
            return _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public int CountUnreadByUserId(int userId)
        {
            return _db.Notifications
                .Count(n => n.UserId == userId && n.Status == "Unread");
        }

        public void Update(Notification entity)
        {
            _db.Notifications.Update(entity);
            _db.SaveChanges();
        }
    }
}