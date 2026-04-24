using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения уведомлений.
    /// </summary>
    public interface INotificationReadRepository
    {
        /// <summary>
        /// Получает уведомления для указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Страница с уведомлениями.</returns>
        Task<PagedResult<NotificationDto>> GetForProfileAsync(Guid profileId, int page, int limit, CancellationToken ct);

        /// <summary>
        /// Получает количество непрочитанных уведомлений для профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Количество непрочитанных уведомлений.</returns>
        Task<int> GetUnreadCountAsync(Guid profileId, CancellationToken ct);
    }
}