using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Обработчик запроса <see cref="GetUnreadCountQuery"/>.
    /// </summary>
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly INotificationReadRepository _notificationReadRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="notificationReadRepository">Репозиторий для чтения уведомлений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetUnreadCountQueryHandler(
            INotificationReadRepository notificationReadRepository,
            ICurrentUserService currentUser)
        {
            _notificationReadRepository = notificationReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            return await _notificationReadRepository.GetUnreadCountAsync(_currentUser.UserId, cancellationToken);
        }
    }
}