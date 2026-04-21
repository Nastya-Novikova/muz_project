using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для отправки уведомлений пользователям (внутренние, email, VK).
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Отправить уведомление профилю с учётом его настроек.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля получателя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Данные для формирования текста уведомления.</param>
        Task SendNotificationToProfileAsync(Guid profileId, NotificationType type, Dictionary<string, object> data);

        /// <summary>
        /// Отправить уведомление пользователю (по userId) с учётом настроек его профиля.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Данные для формирования текста уведомления.</param>
        Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data);
    }
}