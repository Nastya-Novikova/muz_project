using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileGenresChanged"/>.
    /// Записывает интеграционное событие <see cref="ProfileGenresChangedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileGenresChangedHandler : INotificationHandler<ProfileGenresChanged>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileGenresChangedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileGenresChanged notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileGenresChangedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}