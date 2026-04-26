using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="RegisterToEventCommand"/>.
    /// </summary>
    public class RegisterToEventCommandHandler : IRequestHandler<RegisterToEventCommand, Unit>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        public RegisterToEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentUserService currentUser,
            IMusicianProfileRepository profileRepository,
            INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _currentUser = currentUser;
            _profileRepository = profileRepository;
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RegisterToEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
        ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            @event.Register(profile.Id);   // добавляет EventRegistration в owned-коллекцию

            var registration = @event.Registrations.Last();
            await _eventRepository.AttachRegistrationAsync(registration, cancellationToken);

            await _notificationService.SendNotificationToProfileAsync(
                profile,
                NotificationType.EventRegistration,
                new Dictionary<string, object>
                {
                    ["eventTitle"] = @event.Title.Value,
                    ["profileName"] = profile.FullName.Value,
                    ["eventId"] = @event.Id
                });

            return Unit.Value;
        }
    }
}