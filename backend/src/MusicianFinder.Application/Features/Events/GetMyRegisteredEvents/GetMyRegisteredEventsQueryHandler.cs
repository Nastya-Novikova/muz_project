using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Events.DTOs;

namespace MusicianFinder.Application.Features.Events.GetMyRegisteredEvents
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyRegisteredEventsQuery"/>.
    /// </summary>
    public class GetMyRegisteredEventsQueryHandler : IRequestHandler<GetMyRegisteredEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMyRegisteredEventsQueryHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMyRegisteredEventsQueryHandler(
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
        public async Task<PagedResult<EventDto>> Handle(GetMyRegisteredEventsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            var (items, totalCount) = await _eventRepository.GetRegisteredByProfileAsync(profile.Id, request.Page, request.Limit);
            var dtos = _mapper.Map<List<EventDto>>(items);

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(dto.Id);
                dto.IsRegistered = true;
            }

            return new PagedResult<EventDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}