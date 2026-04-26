using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;

namespace MusicianFinder.Application.Interfaces.ReadRepositories
{
    /// <summary>
    /// Репозиторий для чтения предложений о сотрудничестве.
    /// </summary>
    public interface ICollaborationSuggestionReadRepository
    {
        /// <summary>
        /// Получает входящие предложения для указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля получателя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Страница с предложениями.</returns>
        Task<PagedResult<SuggestionDto>> GetReceivedAsync(Guid profileId, int page, int limit, CancellationToken ct);

        /// <summary>
        /// Получает исходящие предложения от указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля отправителя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Страница с предложениями.</returns>
        Task<PagedResult<SuggestionDto>> GetSentAsync(Guid profileId, int page, int limit, CancellationToken ct);

        /// <summary>
        /// Возвращает идентификаторы профилей, которым указанный профиль уже отправил предложения о сотрудничестве.
        /// </summary>
        /// <param name="fromProfileId">Профиль отправителя.</param>
        /// <param name="toProfileIds">Проверяемые профили получателей.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Множество идентификаторов профилей, куда было отправлено предложение.</returns>
        Task<HashSet<Guid>> GetSentSuggestionToProfileIdsAsync(Guid fromProfileId, IEnumerable<Guid> toProfileIds, CancellationToken ct);
    }
}