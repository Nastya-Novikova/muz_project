using MusicianFinder.Application.IntegrationEvents;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Публикатор интеграционных событий во внешнюю шину (например, Kafka).
    /// </summary>
    public interface IExternalBusPublisher
    {
        /// <summary>
        /// Публикует событие во внешнюю шину.
        /// </summary>
        /// <param name="integrationEvent">Интеграционное событие.</param>
        /// <param name="ct">Токен отмены.</param>
        Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default);
    }
}