using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Отправляет внешние уведомления (email, VK) на основе типа события и данных профиля.
    /// Вызывается только вне транзакции.
    /// </summary>
    public interface IExternalNotificationSender
    {
        /// <summary>
        /// Отправляет уведомление профилю с учётом его настроек.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля‑получателя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="data">Данные для подстановки в текст уведомления.</param>
        /// <param name="ct">Токен отмены.</param>
        Task SendAsync(Guid profileId, NotificationType type, IReadOnlyDictionary<string, object> data, CancellationToken cancellationToken = default);
    }
}