using AcademicAppoinment.DTOs.Notification;

namespace AcademicAppoinment.Services.NotificationServices
{
    public interface INotificationService
    {
        NotificationListResponse GetNotifications(
            int userId,
            int page,
            int pageSize,
            bool? isRead);
    }
}
