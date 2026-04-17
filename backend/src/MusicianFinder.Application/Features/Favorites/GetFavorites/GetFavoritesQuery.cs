using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Features.Favorites.DTOs;

namespace MusicianFinder.Application.Features.Favorites.GetFavorites
{
    /// <summary>
    /// Запрос для получения списка избранных профилей.
    /// </summary>
    public class GetFavoritesQuery : IRequest<PagedResult<FavoriteProfileDto>>
    {
        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;
    }
}