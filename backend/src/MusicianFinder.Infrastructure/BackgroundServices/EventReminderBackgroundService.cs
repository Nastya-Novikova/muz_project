using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Фоновый сервис для создания напоминаний о предстоящих мероприятиях.
    /// </summary>
    public class EventReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventReminderBackgroundService> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventReminderBackgroundService"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">Фабрика областей сервисов.</param>
        /// <param name="logger">Логгер.</param>
        public EventReminderBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<EventReminderBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервис напоминаний о мероприятиях запущен.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCreateRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании напоминаний.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckAndCreateRemindersAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MusicianFinderDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var targetStart = DateTime.UtcNow.AddHours(24);
            var startOfDay = targetStart.Date;
            var endOfDay = startOfDay.AddDays(1);

            var events = await dbContext.Events
                .Where(e => !e.IsDeleted &&
                            e.Status == EventStatus.Scheduled &&
                            e.StartDateTime >= startOfDay &&
                            e.StartDateTime < endOfDay)
                .Include(e => e.Registrations)
                .ToListAsync(cancellationToken);

            foreach (var ev in events)
            {
                if (Math.Abs((ev.StartDateTime - targetStart).TotalHours) > 1)
                    continue;

                foreach (var reg in ev.Registrations)
                {
                    var daysLeft = (int)Math.Ceiling((ev.StartDateTime - DateTime.UtcNow).TotalDays);
                    if (daysLeft <= 0) daysLeft = 1;

                    await notificationService.SendNotificationToProfileAsync(
                        reg.ProfileId,
                        NotificationType.EventReminder,
                        new Dictionary<string, object>
                        {
                            ["eventId"] = ev.Id,
                            ["eventTitle"] = ev.Title,
                            ["daysLeft"] = daysLeft
                        });
                }
            }

            _logger.LogInformation("Проверка напоминаний завершена. Обработано мероприятий: {Count}", events.Count);
        }
    }
}