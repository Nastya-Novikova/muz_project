using MediatR;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для отмены регистрации с мероприятия.
    /// </summary>
    public class UnregisterFromEventCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}