using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
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
        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentUserService currentUser,
            IMusicianProfileRepository profileRepository)
        {
            _eventRepository = eventRepository;
            _currentUser = currentUser;
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            var newEvent = new Event(
                new EventTitle(request.Title),
                request.RegionId,
                request.CityId,
                request.Address,
                request.StartDateTime,
                profile.Id,
                request.Description,
                request.EndDateTime,
                request.MaxParticipants);

            _eventRepository.Add(newEvent);
            return newEvent.Id;
        }
    }
}