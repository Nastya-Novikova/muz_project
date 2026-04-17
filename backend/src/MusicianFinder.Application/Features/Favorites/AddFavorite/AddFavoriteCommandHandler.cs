using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Favorites.AddFavorite
{
    /// <summary>
    /// Обработчик команды <see cref="AddFavoriteCommand"/>.
    /// </summary>
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand>
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AddFavoriteCommandHandler"/>.
        /// </summary>
        /// <param name="favoriteRepository">Репозиторий избранного.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public AddFavoriteCommandHandler(
            IFavoriteRepository favoriteRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _favoriteRepository = favoriteRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var targetProfile = await _profileRepository.GetByIdAsync(request.ProfileId);
            if (targetProfile == null)
                throw new NotFoundException(nameof(MusicianProfile), request.ProfileId);

            if (await _favoriteRepository.ExistsAsync(_currentUserService.UserId, request.ProfileId))
                throw new ConflictException("Профиль уже в избранном.");

            var favorite = new Favorite(_currentUserService.UserId, request.ProfileId);
            await _favoriteRepository.AddAsync(favorite);
        }
    }
}