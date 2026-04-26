using MusicianFinder.Application.IntegrationEvents;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис записи интеграционных событий в Outbox.
    /// </summary>
    public interface IOutboxWriter
    {
        /// <summary>
        /// Записывает интеграционное событие в таблицу Outbox в рамках текущей транзакции.
        /// </summary>
        /// <param name="integrationEvent">Интеграционное событие.</param>
        /// <param name="ct">Токен отмены.</param>
        Task WriteAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default);
    }
}