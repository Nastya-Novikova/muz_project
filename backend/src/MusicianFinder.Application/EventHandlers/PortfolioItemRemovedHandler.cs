using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="PortfolioItemRemoved"/>.
    /// Записывает интеграционное событие <see cref="PortfolioItemRemovedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class PortfolioItemRemovedHandler : INotificationHandler<PortfolioItemRemoved>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public PortfolioItemRemovedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(PortfolioItemRemoved notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new PortfolioItemRemovedIntegrationEvent(notification.ProfileId, notification.PortfolioItemId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}