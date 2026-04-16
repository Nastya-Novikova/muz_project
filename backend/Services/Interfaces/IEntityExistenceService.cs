using backend.Models.Classes;
using backend.Models.Common;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Сервис для централизованной проверки существования сущностей.
    /// </summary>
    public interface IEntityExistenceService
    {
        /// <summary>
        /// Проверяет существование пользователя и его музыкального профиля.
        /// </summary>
        Task<Result> ValidateUserWithProfileAsync(Guid userId);

        /// <summary>
        /// Проверяет существование музыкального профиля.
        /// </summary>
        Task<Result> ValidateMusicianProfileAsync(Guid profileId);

        /// <summary>
        /// Проверяет существование города.
        /// </summary>
        Task<Result> ValidateCityAsync(int cityId);

        /// <summary>
        /// Проверяет существование региона.
        /// </summary>
        Task<Result> ValidateRegionAsync(int regionId);

        /// <summary>
        /// Проверяет существование мероприятия.
        /// </summary>
        Task<Result> ValidateEventAsync(Guid eventId);

        /// <summary>
        /// Проверяет, что пользователь ещё не имеет профиля (для создания профиля).
        /// </summary>
        Task<Result> ValidateUserHasNoProfileAsync(Guid userId);

        Task<Result<User>> GetUserWithProfileAsync(Guid userId);
        Task<Result<MusicianProfile>> GetMusicianProfileAsync(Guid profileId);
        Task<Result<City>> GetCityAsync(int cityId);
        Task<Result<Region>> GetRegionAsync(int regionId);
        Task<Result<Event>> GetEventAsync(Guid eventId);

        /// <summary>
        /// Проверяет, что все указанные идентификаторы жанров существуют.
        /// </summary>
        Task<Result> ValidateGenresExistAsync(List<int>? genreIds);

        /// <summary>
        /// Проверяет, что все указанные идентификаторы специальностей существуют.
        /// </summary>
        Task<Result> ValidateSpecialtiesExistAsync(List<int>? specialtyIds);

        /// <summary>
        /// Проверяет, что все указанные идентификаторы целей сотрудничества существуют.
        /// </summary>
        Task<Result> ValidateCollaborationGoalsExistAsync(List<int>? goalIds);
    }
}
