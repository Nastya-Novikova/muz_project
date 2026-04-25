using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="FavoriteAdded"/>.
    /// Записывает интеграционное событие <see cref="FavoriteAddedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class FavoriteAddedHandler : INotificationHandler<DomainEventNotification<FavoriteAdded>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public FavoriteAddedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<FavoriteAdded> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new FavoriteAddedIntegrationEvent(notification.DomainEvent.AddedByProfileId, notification.DomainEvent.TargetProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}