using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileDesiredSpecialtiesChanged"/>.
    /// Записывает интеграционное событие <see cref="ProfileDesiredSpecialtiesChangedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileDesiredSpecialtiesChangedHandler : INotificationHandler<ProfileDesiredSpecialtiesChanged>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileDesiredSpecialtiesChangedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileDesiredSpecialtiesChanged notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileDesiredSpecialtiesChangedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}