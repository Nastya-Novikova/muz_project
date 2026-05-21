using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Notifications;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Notifications
{
    public class GetNotificationSettingsQueryHandlerTests : TestBase
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly GetNotificationSettingsQueryHandler _handler;

        public GetNotificationSettingsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _handler = new GetNotificationSettingsQueryHandler(_profileReadRepository, _currentUser);
        }

        [Fact]
        public async Task Handle_ReturnsSettings()
        {
            var userId = Guid.NewGuid();
            _currentUser.UserId.Returns(userId);
            var profileDto = new ProfileDto { NotifyByEmail = true, NotifyByVk = false };
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profileDto);

            var query = new GetNotificationSettingsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.NotifyByEmail.Should().BeTrue();
            result.NotifyByVk.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ProfileNotFound_ThrowsNotFoundException()
        {
            _currentUser.UserId.Returns(Guid.NewGuid());
            _profileReadRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ProfileDto?)null);

            var query = new GetNotificationSettingsQuery();
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}