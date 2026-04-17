using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Features.Collaborations.DTOs;

namespace MusicianFinder.Application.Features.Collaborations.GetSentSuggestions
{
    /// <summary>
    /// Запрос для получения исходящих предложений о сотрудничестве.
    /// </summary>
    public class GetSentSuggestionsQuery : IRequest<PagedResult<SuggestionDto>>
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