using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Profiles;

namespace MusicianFinder.Application.Queries.Users
{
    /// <summary>
    /// Запрос для получения списка избранных профилей текущего пользователя.
    /// </summary>
    public class GetFavoritesQuery : IRequest<PagedResult<ProfileDto>>
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