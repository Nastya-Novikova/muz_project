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

namespace MusicianFinder.Application.Features.Events.GetMyCreatedEvents
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyCreatedEventsQuery"/>.
    /// </summary>
    public class GetMyCreatedEventsQueryHandler : IRequestHandler<GetMyCreatedEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMyCreatedEventsQueryHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMyCreatedEventsQueryHandler(
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
        public async Task<PagedResult<EventDto>> Handle(GetMyCreatedEventsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            var (items, totalCount) = await _eventRepository.GetCreatedByProfileAsync(profile.Id, request.Page, request.Limit);
            var dtos = _mapper.Map<List<EventDto>>(items);

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(dto.Id);
                dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(dto.Id, profile.Id);
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