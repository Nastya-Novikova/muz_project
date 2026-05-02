using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Обработчик команды <see cref="RemoveFavoriteCommand"/>.
    /// </summary>
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
    {
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="profileProvider">Сервис текущего пользователя.</param>
        public RemoveFavoriteCommandHandler(
            ICurrentProfileProvider profileProvider)
        {
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            profile.RemoveFromFavorites(request.TargetProfileId);
            return Unit.Value;
        }
    }
}