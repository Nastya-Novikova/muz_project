using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileCoreInfoUpdated"/>.
    /// Записывает интеграционное событие <see cref="ProfileCoreInfoUpdatedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileCoreInfoUpdatedHandler : INotificationHandler<DomainEventNotification<ProfileCoreInfoUpdated>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileCoreInfoUpdatedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<ProfileCoreInfoUpdated> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileCoreInfoUpdatedIntegrationEvent(notification.DomainEvent.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}