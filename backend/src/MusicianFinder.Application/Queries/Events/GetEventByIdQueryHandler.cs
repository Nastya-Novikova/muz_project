using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventByIdQuery"/>.
    /// Возвращает мероприятие с флагами IsRegistered и IsCreator для авторизованного пользователя.
    /// </summary>
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        public GetEventByIdQueryHandler(
            IEventReadRepository eventReadRepository,
            ICurrentUserService currentUserService,
            IProfileReadRepository profileReadRepository)
        {
            _eventReadRepository = eventReadRepository;
            _currentUserService = currentUserService;
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _eventReadRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            if (_currentUserService.IsAuthenticated)
            {
                var myProfile = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
                if (myProfile != null)
                {
                    dto.IsCreator = dto.CreatorProfileId == myProfile.Id;
                    dto.IsRegistered = await _eventReadRepository.IsProfileRegisteredAsync(request.EventId, myProfile.Id, cancellationToken);
                }
            }

            return dto;
        }
    }
}