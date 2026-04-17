using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Interfaces;

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
            var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var targetStart = DateTime.UtcNow.AddHours(24);
            var events = await eventRepository.SearchAsync(
                fromDate: targetStart.Date,
                toDate: targetStart.Date.AddDays(1).AddTicks(-1),
                status: EventStatus.Scheduled,
                page: 1,
                limit: 1000);

            foreach (var ev in events.Items)
            {
                if (Math.Abs((ev.StartDateTime - targetStart).TotalHours) > 1)
                    continue;

                var registrations = await eventRepository.GetRegistrationsByEventIdAsync(ev.Id);
                foreach (var reg in registrations)
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

            _logger.LogInformation("Проверка напоминаний завершена. Обработано мероприятий: {Count}", events.Items.Count);
        }
    }
}