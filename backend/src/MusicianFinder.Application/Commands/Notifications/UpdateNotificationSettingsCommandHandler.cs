using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Notifications
{
    public class UpdateNotificationSettingsCommandHandler : IRequestHandler<UpdateNotificationSettingsCommand, Unit>
    {
        private readonly ICurrentProfileProvider _profileProvider;

        public UpdateNotificationSettingsCommandHandler(ICurrentProfileProvider profileProvider)
        {
            _profileProvider = profileProvider;
        }

        public async Task<Unit> Handle(UpdateNotificationSettingsCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            var newNotifyByEmail = request.NotifyByEmail ?? profile.NotifyByEmail;
            var newNotifyByVk = request.NotifyByVk ?? profile.NotifyByVk;

            profile.UpdateNotificationPreferences(newNotifyByEmail, newNotifyByVk);

            return Unit.Value;
        }
    }
}