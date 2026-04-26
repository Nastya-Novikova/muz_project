using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Обработчик команды <see cref="MarkNotificationAsReadCommand"/>.
    /// </summary>
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public MarkNotificationAsReadCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdWithNotificationsAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var notification = profile.Notifications.FirstOrDefault(n => n.Id == request.NotificationId)
                ?? throw new NotFoundException("Уведомление не найдено.");

            notification.MarkAsRead();
            return Unit.Value;
        }
    }
}