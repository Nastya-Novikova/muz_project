using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetGenres
{
    /// <summary>
    /// Запрос для получения списка жанров.
    /// </summary>
    public class GetGenresQuery : IRequest<List<LookupItemDto>>
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
        public bool SortDesc { get; set; } = false;
    }
}