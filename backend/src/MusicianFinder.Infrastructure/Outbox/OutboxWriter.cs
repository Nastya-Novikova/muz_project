using System.Text.Json;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Outbox
{
    /// <summary>
    /// Реализация <see cref="IOutboxWriter"/>, записывающая интеграционные события в таблицу Outbox.
    /// </summary>
    public class OutboxWriter : IOutboxWriter
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="OutboxWriter"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public OutboxWriter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public Task WriteAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default)
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventName = integrationEvent.EventName,
                Version = integrationEvent.Version,
                Payload = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
                OccurredAt = DateTime.UtcNow,
                NextAttemptAt = DateTime.UtcNow,
                RetryCount = 0
            };

            _dbContext.Set<OutboxMessage>().Add(message);
            return Task.CompletedTask;
        }
    }
}