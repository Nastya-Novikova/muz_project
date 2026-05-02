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
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public DeleteProfileCommandHandler(
            ICurrentProfileProvider profileProvider,
            IUserRepository userRepository)
        {
            _profileProvider = profileProvider;
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            var user = await _userRepository.GetByIdAsync(profile.UserId, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            user.ClearMusicianProfile();
            profile.MarkAsDeleted();
            return Unit.Value;
        }
    }
}