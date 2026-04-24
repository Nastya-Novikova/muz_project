using MediatR;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="ProfileCollaborationGoalsChanged"/>.
    /// Записывает интеграционное событие <see cref="ProfileCollaborationGoalsChangedIntegrationEvent"/> в Outbox.
    /// </summary>
    public class ProfileCollaborationGoalsChangedHandler : INotificationHandler<ProfileCollaborationGoalsChanged>
    {
        private readonly IOutboxWriter _outboxWriter;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public ProfileCollaborationGoalsChangedHandler(IOutboxWriter outboxWriter)
        {
            _outboxWriter = outboxWriter;
        }

        /// <inheritdoc />
        public Task Handle(ProfileCollaborationGoalsChanged notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfileCollaborationGoalsChangedIntegrationEvent(notification.ProfileId);
            return _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
        }
    }
}