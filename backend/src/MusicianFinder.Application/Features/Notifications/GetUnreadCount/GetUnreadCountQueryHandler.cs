using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Notifications.GetUnreadCount
{
    /// <summary>
    /// Обработчик запроса <see cref="GetUnreadCountQuery"/>.
    /// </summary>
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetUnreadCountQueryHandler"/>.
        /// </summary>
        /// <param name="notificationRepository">Репозиторий уведомлений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public GetUnreadCountQueryHandler(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            return await _notificationRepository.GetUnreadCountAsync(profile.Id);
        }
    }
}