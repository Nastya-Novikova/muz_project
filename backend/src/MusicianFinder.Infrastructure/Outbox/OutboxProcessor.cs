using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Outbox
{
    /// <summary>
    /// Фоновый процессор, периодически обрабатывающий сообщения из таблицы Outbox.
    /// </summary>
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="OutboxProcessor"/>.
        /// </summary>
        public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке сообщений Outbox");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var registry = scope.ServiceProvider.GetRequiredService<IIntegrationEventTypeRegistry>();
            var publisher = scope.ServiceProvider.GetRequiredService<IExternalBusPublisher>();

            var messages = await db.Set<OutboxMessage>()
                .FromSqlRaw(@"SELECT * FROM ""OutboxMessages""
                              WHERE ""ProcessedAt"" IS NULL AND ""NextAttemptAt"" <= NOW()
                              ORDER BY ""OccurredAt""
                              LIMIT 50 FOR UPDATE SKIP LOCKED")
                .ToListAsync(cancellationToken);

            foreach (var msg in messages)
            {
                try
                {
                    var type = registry.Resolve(msg.EventName, msg.Version);
                    var ev = JsonSerializer.Deserialize(msg.Payload, type) as IIntegrationEvent;
                    if (ev != null)
                    {
                        await publisher.PublishAsync(ev, cancellationToken);
                    }

                    msg.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    msg.RetryCount++;
                    msg.Error = ex.Message;
                    msg.NextAttemptAt = DateTime.UtcNow.AddSeconds(Math.Pow(2, msg.RetryCount));

                    if (msg.RetryCount >= 5)
                    {
                        db.Set<DeadLetter>().Add(new DeadLetter
                        {
                            Id = Guid.NewGuid(),
                            OutboxMessageId = msg.Id,
                            Error = ex.ToString(),
                            MovedAt = DateTime.UtcNow
                        });
                        db.Set<OutboxMessage>().Remove(msg);
                    }
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}