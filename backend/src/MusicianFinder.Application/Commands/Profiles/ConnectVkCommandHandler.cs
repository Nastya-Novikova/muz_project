using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Commands.Profiles
{
    public class ConnectVkCommandHandler : IRequestHandler<ConnectVkCommand, Unit>
    {
        private readonly IVkService _vkService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepository;

        public ConnectVkCommandHandler(IVkService vkService, ICurrentUserService currentUser, IMusicianProfileRepository musicianProfileRepository)
        {
            _vkService = vkService;
            _currentUser = currentUser;
            _profileRepository = musicianProfileRepository;
        }

        public async Task<Unit> Handle(ConnectVkCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль отправителя не найден.");

            await _vkService.ConnectVkAsync(profile.Id, request.Code, request.CodeVerifier, request.DeviceId);
            return Unit.Value;
        }
    }
}