using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Notifications
{
    public class UpdateNotificationSettingsCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Признак согласия на получение email-уведомлений.
        /// </summary>
        public bool? NotifyByEmail { get; set; }

        /// <summary>
        /// Признак согласия на получение уведомлений через ВКонтакте.
        /// </summary>
        public bool? NotifyByVk { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}