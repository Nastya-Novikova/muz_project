using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Заглушка публикатора сообщений во внешнюю шину (логгирует события).
    /// </summary>
    public class ExternalBusPublisher : IExternalBusPublisher
    {
        private readonly ILogger<ExternalBusPublisher> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ExternalBusPublisher"/>.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        public ExternalBusPublisher(ILogger<ExternalBusPublisher> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default)
        {
            _logger.LogInformation("Публикация события {EventName} (v{Version})", integrationEvent.EventName, integrationEvent.Version);
            return Task.CompletedTask;
        }
    }
}