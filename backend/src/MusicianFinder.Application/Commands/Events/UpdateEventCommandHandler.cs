using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Guid>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public UpdateEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            if (@event.CreatorProfileId != profile.Id)
                throw new ForbiddenException("Только создатель может редактировать мероприятие.");

            if (@event.Status != EventStatus.Scheduled)
                throw new DomainException("Редактировать можно только запланированное мероприятие.");

            var newTitle = request.Title != null ? new EventTitle(request.Title) : @event.Title;
            var newDescription = request.Description ?? @event.Description;
            var newRegionId = request.RegionId ?? @event.RegionId;
            var newCityId = request.CityId ?? @event.CityId;
            var newAddress = request.Address ?? @event.Address;
            var newStart = request.StartDateTime ?? @event.StartDateTime;
            var newEnd = request.EndDateTime ?? @event.EndDateTime;
            var newMaxParticipants = request.MaxParticipants ?? @event.MaxParticipants;

            if (newStart <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");

            @event.Update(newTitle, newDescription, newRegionId, newCityId, newAddress,
                          newStart, newEnd, newMaxParticipants, profile.Id);

            return @event.Id;
        }
    }
}