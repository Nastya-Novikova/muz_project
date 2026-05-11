using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
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
        private readonly IExternalNotificationSender _notificationSender;

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
            var notificationSender = scope.ServiceProvider.GetRequiredService<IExternalNotificationSender>();

            var messages = await db.Set<OutboxMessage>()
                .FromSqlRaw(@"SELECT * FROM ""OutboxMessage""
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
                        // Публикация во внешнюю шину (заглушка)
                        await publisher.PublishAsync(ev, cancellationToken);

                        // Отправка email / VK с полными данными
                        await SendExternalNotificationsIfNeeded(ev, notificationSender, db, cancellationToken);
                    }
                    msg.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    msg.RetryCount++;
                    msg.Error = ex.ToString();
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

        private async Task SendExternalNotificationsIfNeeded(IIntegrationEvent ev, IExternalNotificationSender sender, AppDbContext db, CancellationToken cancellationToken)
        {
            switch (ev)
            {
                case UserRegisteredToEventIntegrationEvent reg:
                    var eventEntity = await db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == reg.EventId, cancellationToken);
                    if (eventEntity != null)
                    {
                        var city = await db.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == eventEntity.CityId, cancellationToken);
                        var region = await db.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == eventEntity.RegionId, cancellationToken);
                        var data = new Dictionary<string, object>
                        {
                            ["eventTitle"] = eventEntity.Title.Value,
                            ["address"] = eventEntity.Address,
                            ["cityName"] = city?.Name ?? "Не указан",
                            ["regionName"] = region?.Name ?? "Не указан",
                            ["startDateTime"] = eventEntity.StartDateTime
                        };
                        await sender.SendAsync(reg.ProfileId, NotificationType.EventRegistration, data, cancellationToken);
                    }
                    break;

                case CollaborationSuggestionSentIntegrationEvent sug:
                    var suggestion = await db.CollaborationSuggestions.AsNoTracking()
                        .FirstOrDefaultAsync(cs => cs.Id == sug.SuggestionId, cancellationToken);
                    var fromProfile = await db.MusicianProfiles.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == sug.FromProfileId, cancellationToken);

                    var dataSug = new Dictionary<string, object>
                    {
                        ["fromProfileName"] = fromProfile?.FullName.Value ?? "Пользователь",
                        ["message"] = suggestion?.Message
                    };
                    await sender.SendAsync(sug.ToProfileId, NotificationType.CollaborationReceived, dataSug, cancellationToken);
                    break;
            }
        }
    }
}