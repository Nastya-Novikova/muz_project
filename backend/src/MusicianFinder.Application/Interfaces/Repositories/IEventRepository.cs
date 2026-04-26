using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для записи мероприятий.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Получает мероприятие по идентификатору.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Мероприятие или null, если не найдено.</returns>
        Task<Event?> GetByIdAsync(Guid eventId, CancellationToken ct = default);

        /// <summary>
        /// Добавляет новое мероприятие.
        /// </summary>
        /// <param name="event">Экземпляр мероприятия.</param>
        void Add(Event @event);

        Task AttachRegistrationAsync(EventRegistration registration, CancellationToken ct = default);
    }
}