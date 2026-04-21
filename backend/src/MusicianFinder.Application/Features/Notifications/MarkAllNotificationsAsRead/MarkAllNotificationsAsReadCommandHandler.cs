using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Notifications.MarkAllNotificationsAsRead
{
    /// <summary>
    /// Обработчик команды <see cref="MarkAllNotificationsAsReadCommand"/>.
    /// </summary>
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MarkAllNotificationsAsReadCommandHandler"/>.
        /// </summary>
        /// <param name="notificationRepository">Репозиторий уведомлений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public MarkAllNotificationsAsReadCommandHandler(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            await _notificationRepository.MarkAllAsReadAsync(profile.Id);
            return Unit.Value;
        }
    }
}