using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Команда для отметки всех уведомлений текущего профиля как прочитанных.
    /// </summary>
    public class MarkAllNotificationsAsReadCommand : ICommand<Unit>, IBaseCommand
    {
        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}