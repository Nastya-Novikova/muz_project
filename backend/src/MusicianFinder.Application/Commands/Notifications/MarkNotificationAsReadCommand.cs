using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Команда для отметки одного уведомления как прочитанного.
    /// </summary>
    public class MarkNotificationAsReadCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}