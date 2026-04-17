using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Notifications.GetNotifications;
using MusicianFinder.Application.Features.Notifications.GetUnreadCount;
using MusicianFinder.Application.Features.Notifications.MarkAllNotificationsAsRead;
using MusicianFinder.Application.Features.Notifications.MarkNotificationAsRead;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер уведомлений.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationsController"/>.
        /// </summary>
        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить уведомления.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Отметить уведомление прочитанным.
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _mediator.Send(new MarkNotificationAsReadCommand { NotificationId = id });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Отметить все уведомления прочитанными.
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _mediator.Send(new MarkAllNotificationsAsReadCommand());
            return Ok(new { success = true });
        }

        /// <summary>
        /// Получить количество непрочитанных.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _mediator.Send(new GetUnreadCountQuery());
            return Ok(new { unreadCount = count });
        }
    }
}