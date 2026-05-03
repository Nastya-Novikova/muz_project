using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public RegisterToEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RegisterToEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);


            /*var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");
            @event.Register(profile.Id);
            var registration = @event.Registrations.Last();
            await _eventRepository.AttachRegistrationAsync(registration, cancellationToken);*/


            await _eventRepository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                request.EventId,
                @event => @event.Register(profile.Id),
                cancellationToken);

            return Unit.Value;
        }
    }
}