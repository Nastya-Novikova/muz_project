using MediatR;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для отмены мероприятия.
    /// </summary>
    public class CancelEventCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}