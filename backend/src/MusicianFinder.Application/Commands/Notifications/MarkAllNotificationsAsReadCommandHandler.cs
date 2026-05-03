using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Обработчик команды <see cref="MarkAllNotificationsAsReadCommand"/>.
    /// </summary>
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public MarkAllNotificationsAsReadCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            foreach (var notification in profile.Notifications.Where(n => !n.IsRead))
                notification.MarkAsRead();

            return Unit.Value;
        }
    }
}