using MediatR;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Обработчик запроса <see cref="GetEventByIdQuery"/>.
    /// </summary>
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IEventReadRepository _eventReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventReadRepository">Репозиторий для чтения мероприятий.</param>
        public GetEventByIdQueryHandler(IEventReadRepository eventReadRepository)
        {
            _eventReadRepository = eventReadRepository;
        }

        /// <inheritdoc />
        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _eventReadRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Мероприятие не найдено.");
            return dto;
        }
    }
}