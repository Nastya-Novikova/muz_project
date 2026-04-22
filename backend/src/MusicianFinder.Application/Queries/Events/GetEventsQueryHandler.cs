using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventsQuery"/>.
    /// </summary>
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, PagedResult<EventDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetEventsQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Events
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(nameof(Event.Region))
                .Include(nameof(Event.City))
                .Include(nameof(Event.CreatorProfile))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(e => e.Title.Contains(request.Query) || (e.Description != null && e.Description.Contains(request.Query)));

            if (request.RegionId.HasValue)
                query = query.Where(e => e.RegionId == request.RegionId.Value);

            if (request.CityId.HasValue)
                query = query.Where(e => e.CityId == request.CityId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(e => e.StartDateTime >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(e => e.StartDateTime <= request.ToDate.Value);

            if (request.Status.HasValue)
                query = query.Where(e => e.Status == request.Status.Value);

            if (request.CreatorProfileId.HasValue)
                query = query.Where(e => e.CreatorProfileId == request.CreatorProfileId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var items = await query
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<EventDto>>(items);

            Guid? currentProfileId = null;
            if (_currentUserService.IsAuthenticated)
            {
                var profile = await _dbContext.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken);
                currentProfileId = profile?.Id;
            }

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _dbContext.Events
                    .Where(e => e.Id == dto.Id)
                    .SelectMany(e => e.Registrations)
                    .CountAsync(cancellationToken);
                if (currentProfileId.HasValue)
                {
                    dto.IsRegistered = await _dbContext.Events
                        .Where(e => e.Id == dto.Id)
                        .SelectMany(e => e.Registrations)
                        .AnyAsync(r => r.ProfileId == currentProfileId.Value, cancellationToken);
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

        private static IQueryable<Event> ApplySorting(IQueryable<Event> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "title" => sortDesc ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
                "startdatetime" => sortDesc ? query.OrderByDescending(e => e.StartDateTime) : query.OrderBy(e => e.StartDateTime),
                "createdat" => sortDesc ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt),
                _ => query.OrderByDescending(e => e.StartDateTime)
            };
        }
    }
}