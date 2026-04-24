using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileSpecialtiesChanged"/>.
    /// Записывает интеграционное событие <see cref="ProfileSpecialtiesChangedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileSpecialtiesChangedHandler : INotificationHandler<ProfileSpecialtiesChanged>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileSpecialtiesChangedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileSpecialtiesChanged notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileSpecialtiesChangedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}