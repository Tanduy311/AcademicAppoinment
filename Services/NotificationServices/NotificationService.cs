using AcademicAppoinment.DTOs.Notification;
using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public NotificationListResponse GetNotifications(int userId, int page, int pageSize, bool? isRead)
        {
            // Tạo querry 
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            /* isRead == true => hiển thị danh sách những notification đã đọc rồi
             * isRead == false => hiển thị danh sách những notification chưa đọc rồi
             * isRead == null => hiển thị tất cả những notification chưa đọc + đọc rồi
             */
            if (isRead != null)
            {
                query = query.Where(n => n.IsRead == isRead);
            }

            var totalItems = query.Count();

            var notifications = query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    AppointmentId = n.AppointmentId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();

            return new NotificationListResponse
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,

                TotalPages = (int)Math.Ceiling(
                    (double)totalItems / pageSize
                ),

                Items = notifications
            };
        }
    }
}
