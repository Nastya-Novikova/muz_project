using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для регистрации на мероприятие.
    /// </summary>
    public class RegisterToEventCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}