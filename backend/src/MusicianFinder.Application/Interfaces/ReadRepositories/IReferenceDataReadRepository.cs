using MusicianFinder.Application.DTOs.Metadata;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения справочных данных (города, жанры, специальности и т.д.).
    /// </summary>
    public interface IReferenceDataReadRepository
    {
        /// <summary>
        /// Получает список всех городов.
        /// </summary>
        Task<List<LookupItemDto>> GetCitiesAsync(CancellationToken ct = default);
        /// <summary>
        /// Получает список всех регионов.
        /// </summary>
        Task<List<LookupItemDto>> GetRegionsAsync(CancellationToken ct = default);
        /// <summary>
        /// Получает список всех жанров.
        /// </summary>
        Task<List<LookupItemDto>> GetGenresAsync(CancellationToken ct = default);
        /// <summary>
        /// Получает список всех музыкальных специальностей.
        /// </summary>
        Task<List<LookupItemDto>> GetSpecialtiesAsync(CancellationToken ct = default);
        /// <summary>
        /// Получает список всех целей сотрудничества.
        /// </summary>
        Task<List<LookupItemDto>> GetCollaborationGoalsAsync(CancellationToken ct = default);
    }
}