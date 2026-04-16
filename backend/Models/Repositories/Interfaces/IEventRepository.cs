using backend.Models.Classes;
using backend.Models.DTOs.Events;
using backend.Models.Enums;

namespace backend.Models.Repositories.Interfaces
{
    public interface IEventRepository
    {
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

        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event eventEntity);
        Task UpdateAsync(Event eventEntity);
        Task SoftDeleteAsync(Guid id);

        // Работа с участниками
        Task<bool> IsUserRegisteredAsync(Guid eventId, Guid profileId);
        Task<int> GetRegistrationCountAsync(Guid eventId);
        Task AddRegistrationAsync(EventRegistration registration);
        Task RemoveRegistrationAsync(Guid eventId, Guid profileId);
        Task<List<EventRegistration>> GetRegistrationsByEventIdAsync(Guid eventId);

        // Получение мероприятий пользователя
        Task<(List<Event> Items, int TotalCount)> GetCreatedByProfileAsync(Guid profileId, int page, int limit);
        Task<(List<Event> Items, int TotalCount)> GetRegisteredByProfileAsync(Guid profileId, int page, int limit);
        Task<(List<EventDto> Items, int TotalCount)> GetEventDtosAsync(EventFilterRequest filter, Guid? currentUserId = null);
    }
}
