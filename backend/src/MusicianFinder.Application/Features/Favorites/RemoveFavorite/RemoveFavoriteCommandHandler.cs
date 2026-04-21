using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Favorites.RemoveFavorite
{
    /// <summary>
    /// Обработчик команды <see cref="RemoveFavoriteCommand"/>.
    /// </summary>
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RemoveFavoriteCommandHandler"/>.
        /// </summary>
        /// <param name="favoriteRepository">Репозиторий избранного.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public RemoveFavoriteCommandHandler(
            IFavoriteRepository favoriteRepository,
            ICurrentUserService currentUserService)
        {
            _favoriteRepository = favoriteRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            if (!await _favoriteRepository.ExistsAsync(_currentUserService.UserId, request.ProfileId))
                throw new NotFoundException("Профиль не найден в избранном.");

            await _favoriteRepository.RemoveAsync(_currentUserService.UserId, request.ProfileId);
            return Unit.Value;
        }
    }
}