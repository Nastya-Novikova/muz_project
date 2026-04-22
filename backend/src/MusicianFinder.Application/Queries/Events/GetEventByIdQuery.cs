using MediatR;
using MusicianFinder.Application.DTOs.Events;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Запрос для получения мероприятия по идентификатору.
    /// </summary>
    public class GetEventByIdQuery : IRequest<EventDto>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}