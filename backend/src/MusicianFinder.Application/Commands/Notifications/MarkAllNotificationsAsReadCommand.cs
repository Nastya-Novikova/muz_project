using MediatR;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Команда для отметки всех уведомлений текущего пользователя как прочитанных.
    /// </summary>
    public class MarkAllNotificationsAsReadCommand : IRequest<Unit>
    {
    }
}