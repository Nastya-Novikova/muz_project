using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="CollaborationSuggestionSent"/>.
    /// Записывает интеграционное событие <see cref="CollaborationSuggestionSentIntegrationEvent"/> в Outbox.
    /// </summary>
    public class CollaborationSuggestionSentHandler : INotificationHandler<DomainEventNotification<CollaborationSuggestionSent>>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public CollaborationSuggestionSentHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(DomainEventNotification<CollaborationSuggestionSent> notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new CollaborationSuggestionSentIntegrationEvent(
                notification.DomainEvent.SuggestionId, notification.DomainEvent.FromProfileId, notification.DomainEvent.ToProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}