using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Domain.DomainEvents;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.EventHandlers
{
    public class UserRegisteredToEventNotificationHandler
        : INotificationHandler<DomainEventNotification<UserRegisteredToEvent>>
    {
        private readonly INotificationService _notificationService;
        private readonly IEventReadRepository _eventReadRepository;
        private readonly IProfileReadRepository _profileReadRepository;

        public UserRegisteredToEventNotificationHandler(
            INotificationService notificationService,
            IEventReadRepository eventReadRepository,
            IProfileReadRepository profileReadRepository)
        {
            _notificationService = notificationService;
            _eventReadRepository = eventReadRepository;
            _profileReadRepository = profileReadRepository;
        }

        public async Task Handle(
            DomainEventNotification<UserRegisteredToEvent> notification,
            CancellationToken cancellationToken)
        {
            var evt = await _eventReadRepository.GetByIdAsync(notification.DomainEvent.EventId, cancellationToken);
            if (evt == null) return;

            var profile = await _profileReadRepository.GetByIdAsync(notification.DomainEvent.ProfileId, cancellationToken);
            if (profile == null) return;

            var data = new Dictionary<string, object>
            {
                ["eventTitle"] = evt.Title,
                ["profileName"] = profile.FullName
            };

            // Отправляем уведомление зарегистрировавшемуся участнику
            await _notificationService.SendNotificationToProfileAsync(
                notification.DomainEvent.ProfileId,
                NotificationType.EventRegistration,
                data);
        }
    }
}