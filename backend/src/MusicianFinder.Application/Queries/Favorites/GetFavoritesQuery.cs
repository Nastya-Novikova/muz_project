using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Favorites
{
    /// <summary>
    /// Запрос для получения избранных профилей.
    /// </summary>
    public class GetFavoritesQuery : IQuery<PagedResult<ProfileDto>>
    {
        /// <summary>Номер страницы.</summary>
        public int Page { get; set; } = 1;
        /// <summary>Размер страницы.</summary>
        public int Limit { get; set; } = 20;
    }
}