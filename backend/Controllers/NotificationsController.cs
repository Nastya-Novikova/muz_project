using backend.Models.DTOs.Common;
using backend.Models.DTOs.Notifications;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер уведомлений
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Получить уведомления текущего пользователя за последние 30 дней
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var userId = GetUserId();
            var result = await _notificationService.GetUserNotificationsAsync(userId, page, limit);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Отметить уведомление как прочитанное
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Отметить все уведомления как прочитанные
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Получить количество непрочитанных уведомлений
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        private Guid GetUserId()
        {
            var userIdStr = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }
    }
}
