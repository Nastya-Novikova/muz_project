using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Profiles
{
    public class ConnectVkCommandHandlerTests : TestBase
    {
        private readonly IVkService _vkService;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly ConnectVkCommandHandler _handler;

        public ConnectVkCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _vkService = Substitute.For<IVkService>();
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new ConnectVkCommandHandler(_vkService, _profileProvider);
        }

        [Fact]
        public async Task Handle_ValidCode_SetsVkUserIdAndEnablesVkNotifications()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new ConnectVkCommand { Code = "vk_code", CodeVerifier = "verifier", DeviceId = "device" };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            await _vkService.Received(1).ConnectVkAsync(profile.Id, "vk_code", "verifier", "device");
        }

        [Fact]
        public async Task Handle_InvalidVkCode_ThrowsException()
        {
            var profile = new MusicianProfileBuilder().Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);
            _vkService.ConnectVkAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromException(new System.Exception("Invalid code")));

            var command = new ConnectVkCommand { Code = "bad" };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<System.Exception>().WithMessage("Invalid code");
        }
    }
}