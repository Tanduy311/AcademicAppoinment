using AcademicAppoinment.DTOs.Notification;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IReadOnlyList<NotificationResponseDto>> GetMyNotificationsAsync(ClaimsPrincipal user);
        Task<int> GetUnreadCountAsync(ClaimsPrincipal user);
        Task<bool> MarkAsReadAsync(int notificationId, ClaimsPrincipal user);
    }
}
