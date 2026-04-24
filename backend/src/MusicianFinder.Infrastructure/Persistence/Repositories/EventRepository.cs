using Microsoft.EntityFrameworkCore;
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
            => await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, ct);

        /// <inheritdoc />
        public void Add(Event @event) => _dbContext.Events.Add(@event);
    }
}