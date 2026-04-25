using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="UserRegisteredToEvent"/>.
    /// Записывает интеграционное событие <see cref="UserRegisteredToEventIntegrationEvent"/> в Outbox.
    /// </summary>
    public class UserRegisteredToEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredToEvent>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public UserRegisteredToEventHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<UserRegisteredToEvent> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new UserRegisteredToEventIntegrationEvent(notification.DomainEvent.EventId, notification.DomainEvent.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}