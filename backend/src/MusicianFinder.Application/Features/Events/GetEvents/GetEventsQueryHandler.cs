using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Events.DTOs;

namespace MusicianFinder.Application.Features.Events.GetEvents
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventsQuery"/>.
    /// </summary>
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventsQueryHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetEventsQueryHandler(
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
        public async Task<PagedResult<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _eventRepository.SearchAsync(
                request.Query,
                request.RegionId,
                request.CityId,
                request.FromDate,
                request.ToDate,
                request.Status,
                request.CreatorProfileId,
                request.Page,
                request.Limit,
                request.SortBy,
                request.SortDesc);

            var dtos = _mapper.Map<List<EventDto>>(items);

            Guid? currentProfileId = null;
            if (_currentUserService.IsAuthenticated)
            {
                var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
                currentProfileId = profile?.Id;
            }

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(dto.Id);
                if (currentProfileId.HasValue)
                {
                    dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(dto.Id, currentProfileId.Value);
                    dto.IsCreator = dto.CreatorProfileId == currentProfileId.Value;
                }
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