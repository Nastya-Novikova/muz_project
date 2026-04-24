using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="PortfolioItemAdded"/>.
    /// Записывает интеграционное событие <see cref="PortfolioItemAddedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class PortfolioItemAddedHandler : INotificationHandler<PortfolioItemAdded>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public PortfolioItemAddedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(PortfolioItemAdded notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new PortfolioItemAddedIntegrationEvent(notification.ProfileId, notification.PortfolioItemId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}
