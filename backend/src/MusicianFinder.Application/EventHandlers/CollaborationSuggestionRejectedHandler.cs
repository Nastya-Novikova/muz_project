using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="CollaborationSuggestionRejected"/>.
    /// Записывает интеграционное событие <see cref="CollaborationSuggestionRejectedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class CollaborationSuggestionRejectedHandler : INotificationHandler<CollaborationSuggestionRejected>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public CollaborationSuggestionRejectedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(CollaborationSuggestionRejected notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new CollaborationSuggestionRejectedIntegrationEvent(notification.SuggestionId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}