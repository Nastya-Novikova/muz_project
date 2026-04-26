using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Commands.Favorites
{
    /// <summary>
    /// Обработчик команды <see cref="AddFavoriteCommand"/>.
    /// </summary>
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public AddFavoriteCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            try
            {
                await _profileRepository.AddFavoriteAsync(_currentUser.UserId, request.TargetProfileId, cancellationToken);
            }
            catch (DomainException ex) when (ex.Message.Contains("Этот профиль уже в избранном"))
            {
                throw new ConflictException("Профиль уже добавлен в избранное.");
            }

            return Unit.Value;
        }
    }
}