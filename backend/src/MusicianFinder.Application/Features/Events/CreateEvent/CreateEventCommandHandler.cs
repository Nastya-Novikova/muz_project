using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Events.CreateEvent
{
    /// <summary>
    /// Обработчик команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateEventCommandHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new Common.Exceptions.NotFoundException("Профиль текущего пользователя не найден.");

            var eventEntity = new Event(
                request.Title,
                request.RegionId,
                request.CityId,
                request.Address,
                request.StartDateTime,
                profile.Id,
                request.Description,
                request.EndDateTime,
                request.MaxParticipants);

            await _eventRepository.AddAsync(eventEntity);

            return eventEntity.Id;
        }
    }
}