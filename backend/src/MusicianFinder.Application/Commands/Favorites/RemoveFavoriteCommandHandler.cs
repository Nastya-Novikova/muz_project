using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Обработчик команды <see cref="RemoveFavoriteCommand"/>.
    /// </summary>
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public RemoveFavoriteCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            profile.RemoveFromFavorites(request.TargetProfileId);
            return Unit.Value;
        }
    }
}