using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Enums;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="CancelEventCommand"/>.
    /// </summary>
    public class CancelEventCommandHandler : IRequestHandler<CancelEventCommand, Unit>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public CancelEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(CancelEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            if (@event.CreatorProfileId != profile.Id)
                throw new ForbiddenException("Только создатель может отменить мероприятие.");

            if (@event.Status != EventStatus.Scheduled)
                throw new DomainException("Отменить можно только запланированное мероприятие.");

            @event.Cancel(profile.Id);
            return Unit.Value;
        }
    }
}