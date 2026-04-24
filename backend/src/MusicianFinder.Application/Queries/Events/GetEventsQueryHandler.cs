using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventsQuery"/>.
    /// </summary>
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, PagedResult<EventDto>>
    {
        private readonly IEventReadRepository _eventReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventReadRepository">Репозиторий для чтения мероприятий.</param>
        public GetEventsQueryHandler(IEventReadRepository eventReadRepository)
        {
            _eventReadRepository = eventReadRepository;
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
            return await _eventReadRepository.SearchAsync(filter, cancellationToken);
        }
    }
}