using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Events.RegisterToEvent
{
    /// <summary>
    /// Обработчик команды <see cref="RegisterToEventCommand"/>.
    /// </summary>
    public class RegisterToEventCommandHandler : IRequestHandler<RegisterToEventCommand>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegisterToEventCommandHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="notificationService">Сервис уведомлений.</param>
        public RegisterToEventCommandHandler(
            IEventRepository eventRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task Handle(RegisterToEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            eventEntity.Register(profile.Id);
            await _eventRepository.UpdateAsync(eventEntity);

            // Отправка уведомления создателю мероприятия
            await _notificationService.SendNotificationToProfileAsync(
                eventEntity.CreatorProfileId,
                NotificationType.EventRegistration,
                new Dictionary<string, object>
                {
                    ["eventId"] = eventEntity.Id,
                    ["eventTitle"] = eventEntity.Title,
                    ["participantName"] = profile.FullName
                });
        }
    }
}