using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;

namespace MusicianFinder.Application.Queries.Suggestions
{
    /// <summary>
    /// Запрос для получения входящих предложений о сотрудничестве.
    /// </summary>
    public class GetReceivedSuggestionsQuery : IRequest<PagedResult<SuggestionDto>>
    {
        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Поле сортировки.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки.
        /// </summary>
        public bool SortDesc { get; set; } = true;
    }
}