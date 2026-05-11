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
        private readonly ICurrentProfileProvider _profileProvider;

        public ConnectVkCommandHandler(IVkService vkService, ICurrentProfileProvider profileProvider)
        {
            _vkService = vkService;
            _profileProvider = profileProvider;
        }

        public async Task<Unit> Handle(ConnectVkCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            await _vkService.ConnectVkAsync(profile.Id, request.Code, request.CodeVerifier, request.DeviceId);
            return Unit.Value;
        }
    }
}