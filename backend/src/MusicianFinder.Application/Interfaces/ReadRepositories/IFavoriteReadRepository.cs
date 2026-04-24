using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения избранных профилей.
    /// </summary>
    public interface IFavoriteReadRepository
    {
        /// <summary>
        /// Получает избранные профили указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля, чьё избранное запрашивается.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Страница с избранными профилями.</returns>
        Task<PagedResult<ProfileDto>> GetFavoritesAsync(Guid profileId, int page, int limit, CancellationToken ct);
    }
}