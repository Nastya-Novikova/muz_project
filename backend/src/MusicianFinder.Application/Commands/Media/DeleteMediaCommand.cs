using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Команда для удаления медиа из портфолио текущего пользователя.
    /// </summary>
    public class DeleteMediaCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор медиа.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}