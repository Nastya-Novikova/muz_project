using MediatR;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Команда для отметки одного уведомления как прочитанного.
    /// </summary>
    public class MarkNotificationAsReadCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public Guid NotificationId { get; set; }
    }
}