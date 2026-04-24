using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="FavoriteRemoved"/>.
    /// Записывает интеграционное событие <see cref="FavoriteRemovedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class FavoriteRemovedHandler : INotificationHandler<FavoriteRemoved>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public FavoriteRemovedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(FavoriteRemoved notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new FavoriteRemovedIntegrationEvent(notification.AddedByProfileId, notification.TargetProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}