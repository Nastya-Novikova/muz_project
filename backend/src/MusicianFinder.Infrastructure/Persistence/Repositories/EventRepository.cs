using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с мероприятиями.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public EventRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Region)
                .Include(e => e.City)
                .Include(e => e.CreatorProfile)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        /// <inheritdoc />
        public async Task AddAsync(Event eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity != null)
            {
                eventEntity.MarkAsDeleted();
                _context.Events.Update(eventEntity);
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsUserRegisteredAsync(Guid eventId, Guid profileId)
        {
            return await _context.EventRegistrations
                .AnyAsync(r => r.EventId == eventId && r.ProfileId == profileId);
        }

        /// <inheritdoc />
        public async Task<int> GetRegistrationCountAsync(Guid eventId)
        {
            return await _context.EventRegistrations
                .CountAsync(r => r.EventId == eventId);
        }

        /// <inheritdoc />
        public async Task AddRegistrationAsync(EventRegistration registration)
        {
            await _context.EventRegistrations.AddAsync(registration);
        }

        /// <inheritdoc />
        public async Task RemoveRegistrationAsync(Guid eventId, Guid profileId)
        {
            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ProfileId == profileId);
            if (registration != null)
                _context.EventRegistrations.Remove(registration);
        }

        /// <inheritdoc />
        public async Task<List<EventRegistration>> GetRegistrationsByEventIdAsync(Guid eventId)
        {
            return await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.Profile)
                .ToListAsync();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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