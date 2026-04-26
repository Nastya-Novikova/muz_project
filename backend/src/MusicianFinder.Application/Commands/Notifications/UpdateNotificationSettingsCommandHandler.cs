using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Notifications
{
    public class UpdateNotificationSettingsCommandHandler : IRequestHandler<UpdateNotificationSettingsCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        public UpdateNotificationSettingsCommandHandler(IMusicianProfileRepository profileRepository, ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(UpdateNotificationSettingsCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var newNotifyByEmail = request.NotifyByEmail ?? profile.NotifyByEmail;
            var newNotifyByVk = request.NotifyByVk ?? profile.NotifyByVk;

            profile.UpdateNotificationPreferences(newNotifyByEmail, newNotifyByVk);

            return Unit.Value;
        }
    }
}