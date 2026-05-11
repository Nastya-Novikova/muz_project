using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для мероприятий.
    /// </summary>
    public class EventReadRepository : IEventReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IReferenceDataReadRepository _referenceRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventReadRepository"/>.
        /// </summary>
        public EventReadRepository(AppDbContext dbContext, IMapper mapper, IReferenceDataReadRepository referenceRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _referenceRepository = referenceRepository;
        }

        /// <inheritdoc />
        public async Task<EventDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var @event = await _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

            return @event == null ? null : await EnrichEventDtoAsync(@event, ct);
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> SearchAsync(EventFilterDto filter, CancellationToken ct = default)
        {
            var query = _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Registrations)
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Query))
                query = query.Where(e =>
                    e.Title.Value.Contains(filter.Query) ||
                    (e.Description != null && e.Description.Contains(filter.Query)));

            if (filter.RegionId.HasValue)
                query = query.Where(e => e.RegionId == filter.RegionId.Value);

            if (filter.CityId.HasValue)
                query = query.Where(e => e.CityId == filter.CityId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(e => e.StartDateTime >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(e => e.StartDateTime <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(e => e.Status == Enum.Parse<EventStatus>(filter.Status));

            if (filter.CreatorProfileId.HasValue)
                query = query.Where(e => e.CreatorProfileId == filter.CreatorProfileId.Value);

            var totalCount = await query.CountAsync(ct);

            var events = await query
                .OrderByDescending(e => e.StartDateTime)
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .ToListAsync(ct);

            var items = new List<EventDto>(events.Count);
            foreach (var evt in events)
                items.Add(await EnrichEventDtoAsync(evt, ct));

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
            return await SearchAsync(new EventFilterDto { CreatorProfileId = creatorProfileId, Page = page, Limit = limit }, ct);
        }

        /// <inheritdoc />
        public async Task<PagedResult<EventDto>> GetRegisteredEventsAsync(Guid profileId, int page, int limit, CancellationToken ct = default)
        {
            var query = _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Registrations)
                .Where(e => e.Registrations.Any(r => r.ProfileId == profileId) && !e.IsDeleted)
                .OrderByDescending(e => e.StartDateTime);

            var totalCount = await query.CountAsync(ct);

            var events = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(ct);

            var items = new List<EventDto>(events.Count);
            foreach (var evt in events)
                items.Add(await EnrichEventDtoAsync(evt, ct));

            return new PagedResult<EventDto>
            {
                Items = items,
                Total = totalCount,
                Page = page,
                Limit = limit
            };
        }

        /// <inheritdoc />
        public async Task<bool> IsProfileRegisteredAsync(Guid eventId, Guid profileId, CancellationToken ct = default)
        {
            return await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.Id == eventId && !e.IsDeleted)
                .SelectMany(e => e.Registrations)
                .AnyAsync(r => r.ProfileId == profileId, ct);
        }

        /// <summary>
        /// Обогащает DTO мероприятия: справочники, информация о создателе, количество участников.
        /// </summary>
        private async Task<EventDto> EnrichEventDtoAsync(Domain.Entities.Event evt, CancellationToken ct)
        {
            var dto = _mapper.Map<EventDto>(evt);

            var cities = await _referenceRepository.GetCitiesAsync(ct);
            var regions = await _referenceRepository.GetRegionsAsync(ct);

            dto.City = cities.FirstOrDefault(c => c.Id == evt.CityId) ?? new LookupItemDto();
            dto.Region = regions.FirstOrDefault(r => r.Id == evt.RegionId) ?? new LookupItemDto();

            var creator = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == evt.CreatorProfileId && !p.IsDeleted, ct);

            if (creator != null)
            {
                dto.CreatorFullName = creator.FullName.Value;
                dto.CreatorAvatarUrl = creator.AvatarUrl;
            }

            dto.CurrentParticipants = evt.Registrations.Count;
            return dto;
        }
    }
}