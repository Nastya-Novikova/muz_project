using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="EventUpdated"/>.
    /// Записывает интеграционное событие <see cref="EventUpdatedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class EventUpdatedHandler : INotificationHandler<DomainEventNotification<EventUpdated>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public EventUpdatedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<EventUpdated> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new EventUpdatedIntegrationEvent(notification.DomainEvent.EventId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}