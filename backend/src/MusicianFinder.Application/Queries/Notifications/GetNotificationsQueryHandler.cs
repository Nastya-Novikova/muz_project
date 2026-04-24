using MediatR;
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

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="notificationReadRepository">Репозиторий для чтения уведомлений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetNotificationsQueryHandler(
            INotificationReadRepository notificationReadRepository,
            ICurrentUserService currentUser)
        {
            _notificationReadRepository = notificationReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _notificationReadRepository.GetForProfileAsync(_currentUser.UserId, request.Page, request.Limit, cancellationToken);
        }
    }
}