using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Notifications;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Queries.Notifications;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с уведомлениями текущего пользователя.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : BaseApiController
    {
        /// <summary>
        /// Получить список уведомлений текущего пользователя.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с уведомлениями.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Отметить одно уведомление как прочитанное.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPatch("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            var command = new MarkNotificationAsReadCommand { NotificationId = id };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Отметить все уведомления как прочитанные.
        /// </summary>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPost("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var command = new MarkAllNotificationsAsReadCommand();
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Получить количество непрочитанных уведомлений.
        /// </summary>
        /// <returns>Количество непрочитанных уведомлений.</returns>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await Mediator.Send(new GetUnreadCountQuery());
            return Ok(new { unreadCount = count });
        }
    }
}