using MediatR;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для регистрации на мероприятие.
    /// </summary>
    public class RegisterToEventCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}