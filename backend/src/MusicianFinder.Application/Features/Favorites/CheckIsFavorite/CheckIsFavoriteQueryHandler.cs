using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Favorites.CheckIsFavorite
{
    /// <summary>
    /// Обработчик запроса <see cref="CheckIsFavoriteQuery"/>.
    /// </summary>
    public class CheckIsFavoriteQueryHandler : IRequestHandler<CheckIsFavoriteQuery, bool>
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CheckIsFavoriteQueryHandler"/>.
        /// </summary>
        /// <param name="favoriteRepository">Репозиторий избранного.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CheckIsFavoriteQueryHandler(
            IFavoriteRepository favoriteRepository,
            ICurrentUserService currentUserService)
        {
            _favoriteRepository = favoriteRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<bool> Handle(CheckIsFavoriteQuery request, CancellationToken cancellationToken)
        {
            return await _favoriteRepository.ExistsAsync(_currentUserService.UserId, request.ProfileId);
        }
    }
}