/*using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для отправки уведомлений пользователям (внутренние, email, VK).
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Создаёт и добавляет уведомление для указанного профиля (профиль должен быть загружен с Include Notifications).
        /// </summary>
        /// <param name="profileId">Идентификатор профиля получателя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Данные для формирования текста уведомления.</param>
        Task SendNotificationToProfileAsync(MusicianProfile profile, NotificationType type, Dictionary<string, object> data);

        /// <summary>
        /// Отправляет уведомление пользователю (по userId) с учётом настроек его профиля.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Данные для формирования текста уведомления.</param>
        Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data);
    }
}*/