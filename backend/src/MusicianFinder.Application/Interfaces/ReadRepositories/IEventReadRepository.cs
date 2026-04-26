using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения данных мероприятий.
    /// </summary>
    public interface IEventReadRepository
    {
        /// <summary>
        /// Получает DTO мероприятия по идентификатору.
        /// </summary>
        Task<EventDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Выполняет поиск мероприятий с фильтрацией и пагинацией.
        /// </summary>
        Task<PagedResult<EventDto>> SearchAsync(EventFilterDto filter, CancellationToken ct = default);

        /// <summary>
        /// Получает мероприятия, созданные указанным профилем.
        /// </summary>
        /// <param name="creatorProfileId">Идентификатор профиля создателя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<PagedResult<EventDto>> GetCreatedEventsAsync(Guid creatorProfileId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Получает мероприятия, на которые зарегистрирован указанный профиль.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля участника.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<PagedResult<EventDto>> GetRegisteredEventsAsync(Guid profileId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Проверяет, зарегистрирован ли указанный профиль на данное мероприятие.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>true, если профиль зарегистрирован; иначе false.</returns>
        Task<bool> IsProfileRegisteredAsync(Guid eventId, Guid profileId, CancellationToken ct = default);
    }
}