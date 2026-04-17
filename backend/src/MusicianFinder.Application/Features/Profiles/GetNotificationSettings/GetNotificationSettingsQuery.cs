using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Profiles.GetNotificationSettings
{
    /// <summary>
    /// Запрос для получения настроек уведомлений текущего пользователя.
    /// </summary>
    public class GetNotificationSettingsQuery : IRequest<NotificationSettingsDto>
    {
    }

    /// <summary>
    /// DTO настроек уведомлений.
    /// </summary>
    public class NotificationSettingsDto
    {
        /// <summary>
        /// Уведомления по email.
        /// </summary>
        public bool NotifyByEmail { get; set; }

        /// <summary>
        /// Уведомления по VK.
        /// </summary>
        public bool NotifyByVk { get; set; }
    }
}