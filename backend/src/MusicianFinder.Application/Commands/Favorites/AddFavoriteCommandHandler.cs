using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Обработчик команды <see cref="AddFavoriteCommand"/>.
    /// </summary>
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="profileProvider">Сервис текущего пользователя.</param>
        public AddFavoriteCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentProfileProvider profileProvider,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _profileProvider = profileProvider;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            //var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            //var profile = await _profileRepository.GetByUserIdForEditAsync(_currentUserService.UserId, cancellationToken)
                //?? throw new NotFoundException("Профиль не найден.");

            var userId = _currentUserService.UserId;

            try
            {
                await _profileRepository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                    userId,
                    profile => profile.AddToFavorites(request.TargetProfileId),
                    cancellationToken);

                //profile.AddToFavorites(request.TargetProfileId);
                //await _profileRepository.AddFavoriteAsync(profile.UserId, request.TargetProfileId, cancellationToken);
            }
            catch (DomainException ex) when (ex.Message.Contains("Этот профиль уже в избранном"))
            {
                throw new ConflictException("Профиль уже добавлен в избранное.");
            }

            return Unit.Value;
        }
    }
}