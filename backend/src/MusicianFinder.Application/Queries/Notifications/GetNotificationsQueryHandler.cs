using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Обработчик запроса <see cref="GetNotificationsQuery"/>.
    /// </summary>
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationDto>>
    {
        private readonly INotificationReadRepository _notificationReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="notificationReadRepository">Репозиторий для чтения уведомлений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetNotificationsQueryHandler(
            INotificationReadRepository notificationReadRepository,
            ICurrentUserService currentUser,
            IProfileReadRepository profileReadRepository)
        {
            _notificationReadRepository = notificationReadRepository;
            _currentUser = currentUser;
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                      ?? throw new NotFoundException("Профиль не найден.");
            return await _notificationReadRepository.GetForProfileAsync(profile.Id, request.Page, request.Limit, cancellationToken);
        }
    }
}