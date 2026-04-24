using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileDesiredGenresChanged"/>.
    /// Записывает интеграционное событие <see cref="ProfileDesiredGenresChangedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileDesiredGenresChangedHandler : INotificationHandler<ProfileDesiredGenresChanged>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileDesiredGenresChangedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileDesiredGenresChanged notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileDesiredGenresChangedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}