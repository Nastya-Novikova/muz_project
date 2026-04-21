using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Notifications.MarkNotificationAsRead
{
    /// <summary>
    /// Обработчик команды <see cref="MarkNotificationAsReadCommand"/>.
    /// </summary>
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MarkNotificationAsReadCommandHandler"/>.
        /// </summary>
        /// <param name="notificationRepository">Репозиторий уведомлений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public MarkNotificationAsReadCommandHandler(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification == null)
                throw new NotFoundException(nameof(Notification), request.NotificationId);

            if (notification.ProfileId != profile.Id)
                throw new ForbiddenException("Нет доступа к этому уведомлению.");

            await _notificationRepository.MarkAsReadAsync(request.NotificationId);
            return Unit.Value;
        }
    }
}