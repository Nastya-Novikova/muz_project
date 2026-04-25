using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="UserUnregisteredFromEvent"/>.
    /// Записывает интеграционное событие <see cref="UserUnregisteredFromEventIntegrationEvent"/> в Outbox.
    /// </summary>
    public class UserUnregisteredFromEventHandler : INotificationHandler<DomainEventNotification<UserUnregisteredFromEvent>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public UserUnregisteredFromEventHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<UserUnregisteredFromEvent> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new UserUnregisteredFromEventIntegrationEvent(notification.DomainEvent.EventId, notification.DomainEvent.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}