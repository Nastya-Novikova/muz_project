using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Notifications;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Notifications
{
    public class UpdateNotificationSettingsCommandHandlerTests : TestBase
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly UpdateNotificationSettingsCommandHandler _handler;

        public UpdateNotificationSettingsCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileProvider = Substitute.For<ICurrentProfileProvider>();
            _handler = new UpdateNotificationSettingsCommandHandler(_profileProvider);
        }

        [Fact]
        public async Task Handle_Valid_UpdatesFlags()
        {
            var profile = new MusicianProfileBuilder().WithNotifyByEmail(true).WithNotifyByVk(false).Build();
            _profileProvider.GetCurrentProfileAsync(Arg.Any<CancellationToken>()).Returns(profile);

            var command = new UpdateNotificationSettingsCommand { NotifyByEmail = false, NotifyByVk = true };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            profile.NotifyByEmail.Should().BeFalse();
            profile.NotifyByVk.Should().BeTrue();
        }
    }
}