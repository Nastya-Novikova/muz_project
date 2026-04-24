using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileCreated"/>.
    /// Записывает интеграционное событие <see cref="ProfileCreatedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileCreatedHandler : INotificationHandler<ProfileCreated>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileCreatedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileCreated notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileCreatedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}