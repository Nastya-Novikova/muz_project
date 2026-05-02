using MediatR;
using MusicianFinder.Application.Helpers;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик доменного события <see cref="CollaborationSuggestionSent"/>.
    /// Записывает интеграционное событие <see cref="CollaborationSuggestionSentIntegrationEvent"/> в Outbox и создаёт уведомление для получателя.
    /// </summary>
    public class CollaborationSuggestionSentHandler : INotificationHandler<DomainEventNotification<CollaborationSuggestionSent>>
    {
        private readonly IOutboxWriter _outboxWriter;
        private readonly INotificationWriter _notificationWriter;
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICollaborationSuggestionRepository _suggestionRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public CollaborationSuggestionSentHandler(IOutboxWriter outboxWriter, INotificationWriter notificationWriter, IMusicianProfileRepository musicianProfileRepository, ICollaborationSuggestionRepository suggestionRepository)
        {
            _outboxWriter = outboxWriter;
            _notificationWriter = notificationWriter;
            _profileRepository = musicianProfileRepository;
            _suggestionRepository = suggestionRepository;
        }

        /// <inheritdoc />
        public async Task Handle(DomainEventNotification<CollaborationSuggestionSent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            // 1. Интеграционное событие
            var integrationEvent = new CollaborationSuggestionSentIntegrationEvent(
                domainEvent.SuggestionId, domainEvent.FromProfileId, domainEvent.ToProfileId);
            await _outboxWriter.WriteAsync(integrationEvent, cancellationToken);

            // 2. Уведомление получателю
            var fromProfile = await _profileRepository.GetByUserIdAsync(domainEvent.FromProfileId, cancellationToken);
            if (fromProfile != null)
            {
                var suggestion = await _suggestionRepository.GetByIdAsync(domainEvent.SuggestionId, cancellationToken);
                var suggestionMessage = suggestion?.Message;

                var (title, message) = NotificationTextBuilder.Build(
                    NotificationType.CollaborationReceived,
                    new Dictionary<string, object>
                    {
                        ["fromProfileName"] = fromProfile.FullName.Value,
                        ["message"] = suggestionMessage
                    }
                );

                var notif = new Notification(
                    domainEvent.ToProfileId,
                    NotificationType.CollaborationReceived,
                    title,
                    EntityType.CollaborationSuggestion,
                    domainEvent.SuggestionId,
                    message
                );

                await _notificationWriter.AddAsync(domainEvent.ToProfileId, notif, cancellationToken);
            }
        }
    }
}