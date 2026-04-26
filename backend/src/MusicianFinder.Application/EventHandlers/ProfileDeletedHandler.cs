using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileDeleted"/>.
    /// Записывает интеграционное событие <see cref="ProfileDeletedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileDeletedHandler : INotificationHandler<DomainEventNotification<ProfileDeleted>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileDeletedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<ProfileDeleted> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileDeletedIntegrationEvent(notification.DomainEvent.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}