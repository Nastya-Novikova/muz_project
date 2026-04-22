using MediatR;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Команда для удаления медиа из портфолио текущего пользователя.
    /// </summary>
    public class DeleteMediaCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор медиа.
        /// </summary>
        public Guid MediaId { get; set; }
    }
}