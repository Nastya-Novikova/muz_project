using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Notifications
{
    public class UpdateNotificationSettingsCommand : ICommand<Unit>, IBaseCommand
    {
        public bool? NotifyByEmail { get; set; }
        public bool? NotifyByVk { get; set; }

        public string IdempotencyKey { get; set; } = string.Empty;
    }
}