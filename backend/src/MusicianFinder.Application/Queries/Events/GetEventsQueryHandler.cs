using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventsQuery"/>.
    /// Возвращает список мероприятий с пагинацией и флагами для текущего пользователя.
    /// </summary>
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        public GetEventsQueryHandler(
            IEventReadRepository eventReadRepository,
            ICurrentUserService currentUserService,
            IProfileReadRepository profileReadRepository)
        {
            _eventReadRepository = eventReadRepository;
            _currentUserService = currentUserService;
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var filter = new EventFilterDto
            {
                Query = request.Query,
                RegionId = request.RegionId,
                CityId = request.CityId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Status = request.Status,
                CreatorProfileId = request.CreatorProfileId,
                Page = request.Page,
                Limit = request.Limit,
                SortBy = request.SortBy,
                SortDesc = request.SortDesc
            };
            var result = await _eventReadRepository.SearchAsync(filter, cancellationToken);

            if (_currentUserService.IsAuthenticated)
            {
                var myProfile = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
                if (myProfile != null)
                {
                    foreach (var dto in result.Items)
                    {
                        dto.IsCreator = dto.CreatorProfileId == myProfile.Id;
                        dto.IsRegistered = await _eventReadRepository.IsProfileRegisteredAsync(dto.Id, myProfile.Id, cancellationToken);
                    }
                }
            }

            return result;
        }
    }
}