using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Отвечает исключительно за сохранение объекта <see cref="Notification"/> в базе данных.
    /// Не выполняет никаких внешних вызовов (email, VK).
    /// </summary>
    public interface INotificationWriter
    {
        /// <summary>
        /// Добавляет уведомление в коллекцию профиля и отслеживает его для сохранения.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля‑получателя.</param>
        /// <param name="notification">Экземпляр уведомления.</param>
        /// <param name="ct">Токен отмены.</param>
        Task AddAsync(Guid profileId, Notification notification, CancellationToken ct = default);
    }
}