using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Features.Notifications.DTOs;

namespace MusicianFinder.Application.Features.Notifications.GetNotifications
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