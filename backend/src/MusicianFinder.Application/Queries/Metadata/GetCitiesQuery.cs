using MediatR;
using MusicianFinder.Application.DTOs.Metadata;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Запрос для получения списка городов.
    /// </summary>
    public class GetCitiesQuery : IRequest<List<LookupItemDto>>
    {
        /// <summary>
        /// Поисковый запрос.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Поле сортировки.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки.
        /// </summary>
        public bool SortDesc { get; set; }
    }
}