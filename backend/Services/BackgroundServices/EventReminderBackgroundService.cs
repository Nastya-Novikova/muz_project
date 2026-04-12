using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services.BackgroundServices
{
    /// <summary>
    /// Фоновый сервис для создания напоминаний о предстоящих мероприятиях
    /// </summary>
    public class EventReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventReminderBackgroundService> _logger;

        public EventReminderBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<EventReminderBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервис напоминаний о мероприятиях запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCreateRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании напоминаний о мероприятиях");
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
                status: backend.Models.Enums.EventStatus.Scheduled,
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

                    // Передаём ProfileId напрямую
                    await notificationService.CreateEventReminderAsync(
                        reg.ProfileId,
                        ev.Id,
                        ev.Title,
                        daysLeft);
                }
            }

            _logger.LogInformation("Проверка напоминаний завершена. Обработано мероприятий: {Count}", events.Items.Count);
        }
    }
}
