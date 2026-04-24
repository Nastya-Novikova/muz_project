using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyRegisteredEventsQuery"/>.
    /// </summary>
    public class GetMyRegisteredEventsQueryHandler : IRequestHandler<GetMyRegisteredEventsQuery, PagedResult<EventDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetMyRegisteredEventsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetMyRegisteredEventsQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> Handle(GetMyRegisteredEventsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var query = _dbContext.Events
                .AsNoTracking()
                .Where(e => e.Registrations.Any(r => r.ProfileId == profile.Id) && !e.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(e => e.StartDateTime)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            foreach (var dto in items)
                dto.IsRegistered = true;

            return new PagedResult<EventDto>
            {
                Items = items,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}