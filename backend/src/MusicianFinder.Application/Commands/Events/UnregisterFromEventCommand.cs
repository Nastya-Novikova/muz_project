using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для отмены регистрации с мероприятия.
    /// </summary>
    public class UnregisterFromEventCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}