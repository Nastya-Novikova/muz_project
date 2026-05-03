using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Core.Exceptions;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UnregisterFromEventCommand"/>.
    /// </summary>
    public class UnregisterFromEventCommandHandler : IRequestHandler<UnregisterFromEventCommand, Unit>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public UnregisterFromEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UnregisterFromEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            @event.Unregister(profile.Id);
            return Unit.Value;
        }
    }
}