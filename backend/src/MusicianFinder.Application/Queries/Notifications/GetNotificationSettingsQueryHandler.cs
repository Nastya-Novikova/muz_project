using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Notifications
{
    public class GetNotificationSettingsQueryHandler : IRequestHandler<GetNotificationSettingsQuery, NotificationSettingsDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUser;

        public GetNotificationSettingsQueryHandler(IProfileReadRepository profileReadRepository, ICurrentUserService currentUser)
        {
            _profileReadRepository = profileReadRepository;
            _currentUser = currentUser;
        }

        public async Task<NotificationSettingsDto> Handle(GetNotificationSettingsQuery request, CancellationToken cancellationToken)
        {
            var profileDto = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            return new NotificationSettingsDto
            {
                NotifyByEmail = profileDto.NotifyByEmail,
                NotifyByVk = profileDto.NotifyByVk
            };
        }
    }
}