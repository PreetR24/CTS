using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.API.Extensions;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("notifications")]
    [Authorize]
    public class NotificationsController(INotificationService _notificationservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<NotificationResponseDto>>> GetMyNotifications()
        {
            var userId = User.GetUserId();
            var list = _notificationservice.GetByUserId(userId);
            return ApiResponse<IEnumerable<NotificationResponseDto>>.Ok(list, "Notifications fetched.");
        }

        [HttpGet("unread-count")]
        public ActionResult<ApiResponse<int>> GetUnreadCount()
        {
            var userId = User.GetUserId();
            var count = _notificationservice.GetUnreadCount(userId);
            return ApiResponse<int>.Ok(count, "Unread count fetched.");
        }

        [HttpPatch("{notificationId:int}/read")]
        public ActionResult<ApiResponse<object>> MarkAsRead(int notificationId)
        {
            _notificationservice.MarkAsRead(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification marked as read.");
        }

        [HttpPatch("{notificationId:int}/dismiss")]
        public ActionResult<ApiResponse<object>> Dismiss(int notificationId)
        {
            _notificationservice.Dismiss(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification dismissed.");
        }
    }
}
