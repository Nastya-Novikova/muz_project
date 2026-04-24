using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Запрос для получения уведомлений текущего пользователя.
    /// </summary>
    public class GetNotificationsQuery : IRequest<PagedResult<NotificationDto>>
    {
        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;
    }
}