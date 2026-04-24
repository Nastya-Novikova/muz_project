using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="EventCreated"/>.
    /// Записывает интеграционное событие <see cref="EventCreatedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class EventCreatedHandler : INotificationHandler<EventCreated>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public EventCreatedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(EventCreated notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new EventCreatedIntegrationEvent(notification.EventId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}