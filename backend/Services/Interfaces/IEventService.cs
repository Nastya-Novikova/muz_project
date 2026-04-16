using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с мероприятиями
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Получить ленту мероприятий с фильтрацией и пагинацией.
        /// </summary>
        /// <param name="filter">Параметры фильтрации.</param>
        /// <param name="currentUserId">ID текущего пользователя для определения IsRegistered.</param>
        Task<Result<PagedResult<EventDto>>> GetEventsAsync(EventFilterRequest filter, Guid? currentUserId = null);

        /// <summary>
        /// Получить мероприятие по ID
        /// </summary>
        Task<Result<EventDto>> GetByIdAsync(Guid id, Guid? currentUserId = null);

        /// <summary>
        /// Создать мероприятие
        /// </summary>
        Task<Result<EventDto>> CreateAsync(Guid userId, CreateEventRequest request);

        /// <summary>
        /// Обновить мероприятие (только для создателя)
        /// </summary>
        Task<Result<EventDto>> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request);

        /// <summary>
        /// Отменить мероприятие (только для создателя)
        /// </summary>
        Task<Result> CancelAsync(Guid userId, Guid eventId);

        /// <summary>
        /// Записаться на мероприятие
        /// </summary>
        Task<Result> RegisterAsync(Guid userId, Guid eventId);

        /// <summary>
        /// Отменить запись на мероприятие
        /// </summary>
        Task<Result> UnregisterAsync(Guid userId, Guid eventId);

        /// <summary>
        /// Получить мероприятия, созданные пользователем
        /// </summary>
        Task<Result<PagedResult<EventDto>>> GetMyCreatedEventsAsync(Guid userId, int page, int limit);

        /// <summary>
        /// Получить мероприятия, на которые записан пользователь
        /// </summary>
        Task<Result<PagedResult<EventDto>>> GetMyRegisteredEventsAsync(Guid userId, int page, int limit);

        /// <summary>
        /// Загрузить изображение для мероприятия
        /// </summary>
        Task<Result<string>> UploadImageAsync(Guid userId, Guid eventId, Stream fileStream, string fileName, string contentType);
    }
}
