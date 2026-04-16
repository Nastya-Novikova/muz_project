using backend.Data;
using backend.Exceptions;
using backend.Models.Classes;
using backend.Models.DTOs;
using backend.Models.DTOs.Events;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly MusicianFinderDbContext _context;

        public EventRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Event> Items, int TotalCount)> SearchAsync(
            string? query = null,
            int? regionId = null,
            int? cityId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            EventStatus? status = null,
            Guid? creatorProfileId = null,
            int page = 1,
            int limit = 20,
            string? sortBy = null,
            bool sortDesc = true)
        {
            var queryable = _context.Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.Region)
                .Include(e => e.City)
                .Include(e => e.CreatorProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
                queryable = queryable.Where(e => e.Title.Contains(query) || (e.Description != null && e.Description.Contains(query)));

            if (regionId.HasValue)
                queryable = queryable.Where(e => e.RegionId == regionId.Value);

            if (cityId.HasValue)
                queryable = queryable.Where(e => e.CityId == cityId.Value);

            if (fromDate.HasValue)
                queryable = queryable.Where(e => e.StartDateTime >= fromDate.Value);

            if (toDate.HasValue)
                queryable = queryable.Where(e => e.StartDateTime <= toDate.Value);

            if (status.HasValue)
                queryable = queryable.Where(e => e.Status == status.Value);

            if (creatorProfileId.HasValue)
                queryable = queryable.Where(e => e.CreatorProfileId == creatorProfileId.Value);

            var totalCount = await queryable.CountAsync();

            queryable = ApplySorting(queryable, sortBy, sortDesc);

            var items = await queryable
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Region)
                .Include(e => e.City)
                .Include(e => e.CreatorProfile)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task AddAsync(Event eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            var existing = await _context.Events.FindAsync(eventEntity.Id);

            _context.Events.Update(eventEntity);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);

            eventEntity.IsDeleted = true;
            eventEntity.DeletedAt = DateTime.UtcNow;
            _context.Events.Update(eventEntity);
        }

        public async Task<bool> IsUserRegisteredAsync(Guid eventId, Guid profileId)
        {
            return await _context.EventRegistrations
                .AnyAsync(r => r.EventId == eventId && r.ProfileId == profileId);
        }

        public async Task<int> GetRegistrationCountAsync(Guid eventId)
        {
            return await _context.EventRegistrations
                .CountAsync(r => r.EventId == eventId);
        }

        public async Task AddRegistrationAsync(EventRegistration registration)
        {
            await _context.EventRegistrations.AddAsync(registration);
        }

        public async Task RemoveRegistrationAsync(Guid eventId, Guid profileId)
        {
            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ProfileId == profileId);

            if (registration != null)
                _context.EventRegistrations.Remove(registration);
        }

        public async Task<List<EventRegistration>> GetRegistrationsByEventIdAsync(Guid eventId)
        {
            return await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.Profile)
                .ToListAsync();
        }

        public async Task<(List<Event> Items, int TotalCount)> GetCreatedByProfileAsync(Guid profileId, int page, int limit)
        {
            var queryable = _context.Events
                .Where(e => e.CreatorProfileId == profileId && !e.IsDeleted)
                .Include(e => e.Region)
                .Include(e => e.City)
                .Include(e => e.CreatorProfile);

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<Event> Items, int TotalCount)> GetRegisteredByProfileAsync(Guid profileId, int page, int limit)
        {
            var queryable = _context.EventRegistrations
                .Where(r => r.ProfileId == profileId)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Region)
                .Include(r => r.Event)
                    .ThenInclude(e => e.City)
                .Include(r => r.Event)
                    .ThenInclude(e => e.CreatorProfile)
                .Select(r => r.Event)
                .Where(e => !e.IsDeleted);

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderByDescending(e => e.StartDateTime)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<EventDto> Items, int TotalCount)> GetEventDtosAsync(EventFilterRequest filter, Guid? currentUserId = null)
        {
            var queryable = _context.Events
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Query))
                queryable = queryable.Where(e => e.Title.Contains(filter.Query) || (e.Description != null && e.Description.Contains(filter.Query)));

            if (filter.RegionId.HasValue)
                queryable = queryable.Where(e => e.RegionId == filter.RegionId.Value);

            if (filter.CityId.HasValue)
                queryable = queryable.Where(e => e.CityId == filter.CityId.Value);

            if (filter.FromDate.HasValue)
                queryable = queryable.Where(e => e.StartDateTime >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                queryable = queryable.Where(e => e.StartDateTime <= filter.ToDate.Value);

            if (filter.Status.HasValue)
                queryable = queryable.Where(e => e.Status == filter.Status.Value);

            if (filter.CreatorProfileId.HasValue)
                queryable = queryable.Where(e => e.CreatorProfileId == filter.CreatorProfileId.Value);

            var totalCount = await queryable.CountAsync();

            IOrderedQueryable<Event> orderedQuery = filter.SortBy?.ToLower() switch
            {
                "title" => filter.SortDesc ? queryable.OrderByDescending(e => e.Title) : queryable.OrderBy(e => e.Title),
                "startdatetime" => filter.SortDesc ? queryable.OrderByDescending(e => e.StartDateTime) : queryable.OrderBy(e => e.StartDateTime),
                "createdat" => filter.SortDesc ? queryable.OrderByDescending(e => e.CreatedAt) : queryable.OrderBy(e => e.CreatedAt),
                _ => filter.SortDesc ? queryable.OrderByDescending(e => e.StartDateTime) : queryable.OrderBy(e => e.StartDateTime)
            };

            var items = await orderedQuery
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl,
                    Region = new LookupItemDto { Id = e.Region.Id, Name = e.Region.Name, LocalizedName = e.Region.LocalizedName },
                    City = new LookupItemDto { Id = e.City.Id, Name = e.City.Name, LocalizedName = e.City.LocalizedName },
                    Address = e.Address,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    MaxParticipants = e.MaxParticipants,
                    CurrentParticipants = e.Registrations.Count(),
                    Status = e.Status,
                    CreatorProfileId = e.CreatorProfileId,
                    CreatorFullName = e.CreatorProfile.FullName,
                    CreatorAvatarUrl = e.CreatorProfile.AvatarUrl,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    IsRegistered = currentUserId.HasValue
                        ? e.Registrations.Any(r => r.ProfileId == currentUserId.Value)
                        : false
                })
                .ToListAsync();

            return (items, totalCount);
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
