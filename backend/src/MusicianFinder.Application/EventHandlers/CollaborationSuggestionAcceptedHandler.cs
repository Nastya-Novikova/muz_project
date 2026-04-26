using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="CollaborationSuggestionAccepted"/>.
    /// Записывает интеграционное событие <see cref="CollaborationSuggestionAcceptedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class CollaborationSuggestionAcceptedHandler : INotificationHandler<DomainEventNotification<CollaborationSuggestionAccepted>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public CollaborationSuggestionAcceptedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<CollaborationSuggestionAccepted> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new CollaborationSuggestionAcceptedIntegrationEvent(notification.DomainEvent.SuggestionId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}