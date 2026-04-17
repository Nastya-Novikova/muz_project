using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Events.UpdateEvent
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateEventCommandHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public UpdateEventCommandHandler(
            IEventRepository eventRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            eventEntity.Update(
                request.Title ?? eventEntity.Title,
                request.Description ?? eventEntity.Description,
                request.RegionId ?? eventEntity.RegionId,
                request.CityId ?? eventEntity.CityId,
                request.Address ?? eventEntity.Address,
                request.StartDateTime ?? eventEntity.StartDateTime,
                request.EndDateTime ?? eventEntity.EndDateTime,
                request.MaxParticipants ?? eventEntity.MaxParticipants,
                profile.Id);

            await _eventRepository.UpdateAsync(eventEntity);
        }
    }
}