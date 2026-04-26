using MediatR;
using MusicianFinder.Application.Core.Exceptions;
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
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="notificationReadRepository">Репозиторий для чтения уведомлений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetUnreadCountQueryHandler(
            INotificationReadRepository notificationReadRepository,
            ICurrentUserService currentUser,
            IProfileReadRepository profileReadRepository)
        {
            _notificationReadRepository = notificationReadRepository;
            _currentUser = currentUser;
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                      ?? throw new NotFoundException("Профиль не найден.");
            return await _notificationReadRepository.GetUnreadCountAsync(profile.Id, cancellationToken);
        }
    }
}