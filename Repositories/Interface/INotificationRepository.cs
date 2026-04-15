using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface INotificationRepository
    {
        void Add(Notification entity);
        Notification? GetById(int notificationId);
        IEnumerable<Notification> GetByUserId(int userId);
        int CountUnreadByUserId(int userId);
        void Update(Notification entity);
    }
}