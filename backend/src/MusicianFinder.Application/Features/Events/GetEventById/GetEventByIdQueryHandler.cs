using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Events.DTOs;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Events.GetEventById
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventByIdQuery"/>.
    /// </summary>
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventByIdQueryHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetEventByIdQueryHandler(
            IEventRepository eventRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                throw new NotFoundException(nameof(Event), request.EventId);

            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(request.EventId);

            if (_currentUserService.IsAuthenticated)
            {
                var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
                if (profile != null)
                {
                    dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(request.EventId, profile.Id);
                }
            }

            return dto;
        }
    }
}