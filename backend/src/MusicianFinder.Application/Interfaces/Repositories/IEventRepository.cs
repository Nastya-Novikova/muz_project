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

        /// <summary>
        /// Выполняет доменную операцию, создающую новую owned-сущность в мероприятии,
        /// и гарантирует, что она будет сохранена как новая запись.
        /// </summary>
        /// <typeparam name="T">Тип owned-сущности (например, EventRegistration).</typeparam>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="domainOperation">
        /// Делегат, принимающий мероприятие и возвращающий созданную owned-сущность.
        /// </param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Задача, завершающаяся после выполнения операции.</returns>
        Task ExecuteAndTrackNewOwnedAsync<T>(
            Guid eventId,
            Func<Event, T> domainOperation,
            CancellationToken ct = default)
            where T : class;

        /// <summary>
        /// Помечает переданную регистрацию как новую для вставки в базу данных.
        /// </summary>
        /// <param name="registration">Регистрация.</param>
        /// <param name="ct">Токен отмены.</param>
        Task AttachRegistrationAsync(EventRegistration registration, CancellationToken ct = default);
    }
}