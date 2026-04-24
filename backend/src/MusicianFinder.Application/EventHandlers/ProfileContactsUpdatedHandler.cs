using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileContactsUpdated"/>.
    /// Записывает интеграционное событие <see cref="ProfileContactsUpdatedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileContactsUpdatedHandler : INotificationHandler<ProfileContactsUpdated>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileContactsUpdatedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileContactsUpdated notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileContactsUpdatedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}