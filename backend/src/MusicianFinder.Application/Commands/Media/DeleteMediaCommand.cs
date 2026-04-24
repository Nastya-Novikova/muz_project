using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Команда для удаления медиа из портфолио.
    /// </summary>
    public class DeleteMediaCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор медиа.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}