using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteProfileCommand"/>.
    /// </summary>
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public DeleteProfileCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser,
            IUserRepository userRepository)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            user.ClearMusicianProfile();
            profile.MarkAsDeleted();
            return Unit.Value;
        }
    }
}