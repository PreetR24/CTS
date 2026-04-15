using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface INotificationService
    {
        IEnumerable<NotificationResponseDto> GetByUserId(int userId);
        int GetUnreadCount(int userId);
        void MarkAsRead(int notificationId);
        void Dismiss(int notificationId);
        void Create(int userId, string message, string category);
    }
}
