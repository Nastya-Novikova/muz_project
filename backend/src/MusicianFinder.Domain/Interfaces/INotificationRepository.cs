using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с уведомлениями.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Получить уведомления указанного профиля с пагинацией.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="fromDate">Фильтр по дате создания (начиная с).</param>
        /// <returns>Кортеж: список уведомлений и общее количество.</returns>
        Task<(List<Notification> Items, int TotalCount)> GetByProfileIdAsync(Guid profileId, int page, int limit, DateTime? fromDate = null);

        /// <summary>
        /// Получить уведомление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <returns>Уведомление или null, если не найдено.</returns>
        Task<Notification?> GetByIdAsync(Guid id);

        /// <summary>
        /// Добавить новое уведомление.
        /// </summary>
        /// <param name="notification">Уведомление для добавления.</param>
        Task AddAsync(Notification notification);

        /// <summary>
        /// Отметить уведомление как прочитанное.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        Task MarkAsReadAsync(Guid id);

        /// <summary>
        /// Отметить все уведомления профиля как прочитанные.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        Task MarkAllAsReadAsync(Guid profileId);

        /// <summary>
        /// Получить количество непрочитанных уведомлений профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Количество непрочитанных уведомлений.</returns>
        Task<int> GetUnreadCountAsync(Guid profileId);
    }
}