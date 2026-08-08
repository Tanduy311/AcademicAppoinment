using AcademicAppoinment.Services.NotificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetNotifications(int page = 1, int pageSize = 10, bool? isRead = null)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(
                    "Page and pageSize must be greater than 0"
                );
            }

            var result =
                _notificationService.GetNotifications(
                    userId,
                    page,
                    pageSize,
                    isRead
                );

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = _notificationService.MarkAsRead(
                id,
                userId
            );

            if (result == false)
            {
                return NotFound("Notification was not found");
            }

            return Ok(new
            {
                message = "Notification marked as read"
            });
        }

        [Authorize]
        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var updatedCount =
                _notificationService.MarkAllAsRead(userId);

            return Ok(new
            {
                message = "All notifications marked as read",
                updatedCount = updatedCount
            });
        }
    }
}
