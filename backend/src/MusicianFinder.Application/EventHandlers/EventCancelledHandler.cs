using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="EventCancelled"/>.
    /// Записывает интеграционное событие <see cref="EventCancelledIntegrationEvent"/> в Outbox.
    /// </summary>
    public class EventCancelledHandler : INotificationHandler<EventCancelled>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public EventCancelledHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(EventCancelled notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new EventCancelledIntegrationEvent(notification.EventId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}