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
        /// Получает избранные профили указанного профиля с пагинацией.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля, чьё избранное запрашивается.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Страница с избранными профилями.</returns>
        Task<PagedResult<ProfileDto>> GetFavoritesAsync(Guid profileId, int page, int limit, CancellationToken ct);

        /// <summary>
        /// Возвращает набор идентификаторов профилей, которые добавлены в избранное указанным профилем.
        /// </summary>
        /// <param name="addedByProfileId">Профиль, который добавлял в избранное.</param>
        /// <param name="targetProfileIds">Список проверяемых профилей.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Множество идентификаторов профилей, находящихся в избранном.</returns>
        Task<HashSet<Guid>> GetFavoritedProfileIdsAsync(Guid addedByProfileId, IEnumerable<Guid> targetProfileIds, CancellationToken ct);
    }
}