using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Profiles;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения данных музыкальных профилей.
    /// </summary>
    public interface IProfileReadRepository
    {
        /// <summary>
        /// Получает полный DTO профиля по идентификатору профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<ProfileDto?> GetByIdAsync(Guid profileId, CancellationToken ct = default);

        /// <summary>
        /// Получает DTO профиля по идентификатору пользователя-владельца.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<ProfileDto?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Выполняет поиск профилей с фильтрацией и пагинацией.
        /// </summary>
        /// <param name="query">Параметры поиска.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<PagedResult<ProfileDto>> SearchAsync(SearchProfilesQuery query, CancellationToken ct = default);

        /// <summary>
        /// Получает медиа-контент указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        Task<MediaDto?> GetMediaAsync(Guid profileId, CancellationToken ct = default);
    }
}