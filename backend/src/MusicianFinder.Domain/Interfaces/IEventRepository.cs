using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с мероприятиями.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Выполнить поиск мероприятий с фильтрацией и пагинацией.
        /// </summary>
        /// <param name="query">Поисковый запрос по названию и описанию.</param>
        /// <param name="regionId">Фильтр по региону.</param>
        /// <param name="cityId">Фильтр по городу.</param>
        /// <param name="fromDate">Фильтр по дате начала (с).</param>
        /// <param name="toDate">Фильтр по дате начала (по).</param>
        /// <param name="status">Фильтр по статусу мероприятия.</param>
        /// <param name="creatorProfileId">Фильтр по создателю.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="sortBy">Поле сортировки (title, startdatetime, createdat).</param>
        /// <param name="sortDesc">Направление сортировки.</param>
        /// <returns>Кортеж: список мероприятий и общее количество.</returns>
        Task<(List<Event> Items, int TotalCount)> SearchAsync(
            string? query = null,
            int? regionId = null,
            int? cityId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            EventStatus? status = null,
            Guid? creatorProfileId = null,
            int page = 1,
            int limit = 20,
            string? sortBy = null,
            bool sortDesc = true);

        /// <summary>
        /// Получить мероприятие по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <returns>Мероприятие или null, если не найдено.</returns>
        Task<Event?> GetByIdAsync(Guid id);

        /// <summary>
        /// Добавить новое мероприятие.
        /// </summary>
        /// <param name="eventEntity">Мероприятие для добавления.</param>
        Task AddAsync(Event eventEntity);

        /// <summary>
        /// Обновить существующее мероприятие.
        /// </summary>
        /// <param name="eventEntity">Мероприятие с обновлёнными данными.</param>
        Task UpdateAsync(Event eventEntity);

        /// <summary>
        /// Мягко удалить мероприятие по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        Task SoftDeleteAsync(Guid id);

        /// <summary>
        /// Проверить, зарегистрирован ли указанный профиль на мероприятие.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>true, если профиль зарегистрирован.</returns>
        Task<bool> IsUserRegisteredAsync(Guid eventId, Guid profileId);

        /// <summary>
        /// Получить количество зарегистрированных участников мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <returns>Количество участников.</returns>
        Task<int> GetRegistrationCountAsync(Guid eventId);

        /// <summary>
        /// Добавить регистрацию на мероприятие.
        /// </summary>
        /// <param name="registration">Регистрация для добавления.</param>
        Task AddRegistrationAsync(EventRegistration registration);

        /// <summary>
        /// Удалить регистрацию профиля с мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        Task RemoveRegistrationAsync(Guid eventId, Guid profileId);

        /// <summary>
        /// Получить все регистрации для указанного мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <returns>Список регистраций.</returns>
        Task<List<EventRegistration>> GetRegistrationsByEventIdAsync(Guid eventId);

        /// <summary>
        /// Получить мероприятия, созданные указанным профилем, с пагинацией.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля создателя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <returns>Кортеж: список мероприятий и общее количество.</returns>
        Task<(List<Event> Items, int TotalCount)> GetCreatedByProfileAsync(Guid profileId, int page, int limit);

        /// <summary>
        /// Получить мероприятия, на которые зарегистрирован указанный профиль, с пагинацией.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля участника.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <returns>Кортеж: список мероприятий и общее количество.</returns>
        Task<(List<Event> Items, int TotalCount)> GetRegisteredByProfileAsync(Guid profileId, int page, int limit);
    }
}