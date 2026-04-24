using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для мероприятий.
    /// </summary>
    public class EventReadRepository : IEventReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventReadRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public EventReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<EventDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.Id == id && !e.IsDeleted)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> SearchAsync(EventFilterDto filter, CancellationToken ct = default)
        {
            var query = _dbContext.Events.AsNoTracking().Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(filter.Query))
                query = query.Where(e => e.Title.Value.Contains(filter.Query));

            if (filter.RegionId.HasValue)
                query = query.Where(e => e.RegionId == filter.RegionId.Value);

            if (filter.CityId.HasValue)
                query = query.Where(e => e.CityId == filter.CityId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(e => e.StartDateTime >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(e => e.StartDateTime <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(e => e.Status == Enum.Parse<Domain.Enums.EventStatus>(filter.Status));

            if (filter.CreatorProfileId.HasValue)
                query = query.Where(e => e.CreatorProfileId == filter.CreatorProfileId.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(e => e.StartDateTime)
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<EventDto>
            {
                Items = items,
                Total = totalCount,
                Page = filter.Page,
                Limit = filter.Limit
            };
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> GetCreatedEventsAsync(Guid creatorProfileId, int page, int limit, CancellationToken ct = default)
        {
            var query = _dbContext.Events.AsNoTracking()
                .Where(e => e.CreatorProfileId == creatorProfileId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<EventDto> { Items = items, Total = totalCount, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> GetRegisteredEventsAsync(Guid profileId, int page, int limit, CancellationToken ct = default)
        {
            var query = _dbContext.Events.AsNoTracking()
                .Where(e => e.Registrations.Any(r => r.ProfileId == profileId) && !e.IsDeleted)
                .OrderByDescending(e => e.StartDateTime);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<EventDto> { Items = items, Total = totalCount, Page = page, Limit = limit };
        }
    }
}