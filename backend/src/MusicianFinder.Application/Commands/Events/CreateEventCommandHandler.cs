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
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

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