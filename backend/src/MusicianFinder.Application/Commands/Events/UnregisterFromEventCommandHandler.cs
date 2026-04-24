using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UnregisterFromEventCommand"/>.
    /// </summary>
    public class UnregisterFromEventCommandHandler : IRequestHandler<UnregisterFromEventCommand, Unit>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        public UnregisterFromEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentUserService currentUser,
            IMusicianProfileRepository profileRepository)
        {
            _eventRepository = eventRepository;
            _currentUser = currentUser;
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UnregisterFromEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Мероприятие не найдено.");

            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            @event.Unregister(profile.Id);
            return Unit.Value;
        }
    }
}