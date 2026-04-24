using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Команда для отметки всех уведомлений текущего пользователя как прочитанных.
    /// </summary>
    public class MarkAllNotificationsAsReadCommand : IRequest<Unit>, IBaseCommand
    {
        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}