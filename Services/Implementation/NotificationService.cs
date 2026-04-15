using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class NotificationService(
            INotificationRepository _notifRepo,
            IUnitOfWork _uow) : INotificationService
    {
        public IEnumerable<NotificationResponseDto> GetByUserId(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid userId.");
            var items = _notifRepo.GetByUserId(userId);
            return items.Select(Map).ToList();
        }

        public int GetUnreadCount(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid userId.");
            return _notifRepo.CountUnreadByUserId(userId);
        }

        public void MarkAsRead(int notificationId)
        {
            var entity = _notifRepo.GetById(notificationId);
            if (entity == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");
            entity.Status = "Read";
            _notifRepo.Update(entity);
            _uow.SaveChanges();
        }

        public void Dismiss(int notificationId)
        {
            var entity = _notifRepo.GetById(notificationId);
            if (entity == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");
            entity.Status = "Dismissed";
            _notifRepo.Update(entity);
            _uow.SaveChanges();
        }

        public void Create(int userId, string message, string category)
        {
            var entity = new Notification
            {
                UserId = userId,
                Message = message,
                Category = category,
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            };
            _notifRepo.Add(entity);
            _uow.SaveChanges();
        }

        private static NotificationResponseDto Map(Notification n) => new()
        {
            NotificationId = n.NotificationId,
            UserId = n.UserId,
            Message = n.Message,
            Category = n.Category,
            Status = n.Status,
            CreatedDate = n.CreatedDate
        };
    }
}