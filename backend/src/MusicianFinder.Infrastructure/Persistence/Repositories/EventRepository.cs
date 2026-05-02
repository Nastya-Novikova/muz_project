using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация репозитория для записи мероприятий.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public EventRepository(AppDbContext dbContext) => _dbContext = dbContext;

        /// <inheritdoc />
        public async Task<Event?> GetByIdAsync(Guid eventId, CancellationToken ct = default)
            => await _dbContext.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, ct);

        /// <inheritdoc />
        public void Add(Event @event) => _dbContext.Events.Add(@event);

        public Task AttachRegistrationAsync(EventRegistration registration, CancellationToken ct = default)
        {
            _dbContext.Entry(registration).State = EntityState.Added;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task ExecuteAndTrackNewOwnedAsync<T>(
            Guid eventId,
            Func<Event, T> domainOperation,
            CancellationToken ct = default)
            where T : class
        {
            var @event = await GetByIdAsync(eventId, ct)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var newEntity = domainOperation(@event);
            _dbContext.Entry(newEntity).State = EntityState.Added;
        }

    }
}