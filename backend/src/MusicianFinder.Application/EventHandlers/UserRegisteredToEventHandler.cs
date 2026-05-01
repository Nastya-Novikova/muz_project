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
    /// Обработчик доменного события <see cref="UserRegisteredToEvent"/>.
    /// Записывает интеграционное событие <see cref="UserRegisteredToEventIntegrationEvent"/> в Outbox и создаёт уведомление в БД для зарегистрированного профиля.
    /// </summary>
    public class UserRegisteredToEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredToEvent>>
    {
        private readonly IOutboxWriter _outboxWriter;
        private readonly INotificationWriter _notificationWriter;
        private readonly IEventRepository _eventRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="outboxWriter">Сервис записи в Outbox.</param>
        public UserRegisteredToEventHandler(IOutboxWriter outboxWriter, INotificationWriter notificationWriter, IEventRepository eventRepository)
        {
            _outboxWriter = outboxWriter;
            _notificationWriter = notificationWriter;
            _eventRepository = eventRepository;
        }

        /// <inheritdoc />
        public async Task Handle(DomainEventNotification<UserRegisteredToEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            // 1. Интеграционное событие
            var integrationEvent = new UserRegisteredToEventIntegrationEvent(domainEvent.EventId, domainEvent.ProfileId);
            await _outboxWriter.WriteAsync(integrationEvent, cancellationToken);

            // 2. Внутреннее уведомление участнику
            var @event = await _eventRepository.GetByIdAsync(domainEvent.EventId, cancellationToken);
            if (@event != null)
            {
                var (title, message) = NotificationTextBuilder.Build(
                    NotificationType.EventRegistration,
                    new Dictionary<string, object> { ["eventTitle"] = @event.Title.Value }
                );

                var notif = new Notification(
                    domainEvent.ProfileId,
                    NotificationType.EventRegistration,
                    title,
                    EntityType.Event,
                    domainEvent.EventId,
                    message
                );

                await _notificationWriter.AddAsync(domainEvent.ProfileId, notif, cancellationToken);
            }
        }
    }
}