namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис проверки существования элементов справочников.
    /// </summary>
    public interface IReferenceDataValidationService
    {
        /// <summary>Проверяет существование города с указанным идентификатором.</summary>
        Task<bool> CityExistsAsync(int cityId, CancellationToken ct = default);
        /// <summary>Проверяет существование региона.</summary>
        Task<bool> RegionExistsAsync(int regionId, CancellationToken ct = default);
        /// <summary>Проверяет существование жанра.</summary>
        Task<bool> GenreExistsAsync(int genreId, CancellationToken ct = default);
        /// <summary>Проверяет существование музыкальной специальности.</summary>
        Task<bool> SpecialtyExistsAsync(int specialtyId, CancellationToken ct = default);
        /// <summary>Проверяет существование цели сотрудничества.</summary>
        Task<bool> CollaborationGoalExistsAsync(int goalId, CancellationToken ct = default);
    }
}