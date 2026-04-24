using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Favorites
{
    /// <summary>
    /// Обработчик запроса <see cref="GetFavoritesQuery"/>.
    /// </summary>
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PagedResult<ProfileDto>>
    {
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="favoriteReadRepository">Репозиторий для чтения избранного.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetFavoritesQueryHandler(IFavoriteReadRepository favoriteReadRepository, ICurrentUserService currentUser)
        {
            _favoriteReadRepository = favoriteReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            return await _favoriteReadRepository.GetFavoritesAsync(_currentUser.UserId, request.Page, request.Limit, cancellationToken);
        }
    }
}