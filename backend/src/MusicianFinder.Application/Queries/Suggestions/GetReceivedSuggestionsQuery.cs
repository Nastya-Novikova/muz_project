using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Suggestions
{
    /// <summary>
    /// Запрос для получения входящих предложений о сотрудничестве.
    /// </summary>
    public class GetReceivedSuggestionsQuery : IQuery<PagedResult<SuggestionDto>>
    {
        /// <summary>Номер страницы.</summary>
        public int Page { get; set; } = 1;
        /// <summary>Размер страницы.</summary>
        public int Limit { get; set; } = 20;
    }
}