using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyRegisteredEventsQuery"/>.
    /// </summary>
    public class GetMyRegisteredEventsQueryHandler : IRequestHandler<GetMyRegisteredEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventReadRepository">Репозиторий для чтения мероприятий.</param>
        /// <param name="profileReadRepository">Репозиторий для чтения профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetMyRegisteredEventsQueryHandler(
            IEventReadRepository eventReadRepository,
            IProfileReadRepository profileReadRepository,
            ICurrentUserService currentUser)
        {
            _eventReadRepository = eventReadRepository;
            _profileReadRepository = profileReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> Handle(GetMyRegisteredEventsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            return await _eventReadRepository.GetRegisteredEventsAsync(profile.Id, request.Page, request.Limit, cancellationToken);
        }
    }
}