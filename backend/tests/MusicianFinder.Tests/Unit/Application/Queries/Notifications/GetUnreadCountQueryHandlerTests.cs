using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
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
    public class GetUnreadCountQueryHandlerTests : TestBase
    {
        private readonly INotificationReadRepository _notificationReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetUnreadCountQueryHandler _handler;

        public GetUnreadCountQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _notificationReadRepository = Substitute.For<INotificationReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetUnreadCountQueryHandler(_notificationReadRepository, _currentUser, _profileReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsUnreadCount()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUser.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);
            _notificationReadRepository.GetUnreadCountAsync(profile.Id, Arg.Any<CancellationToken>()).Returns(5);

            var query = new GetUnreadCountQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(5);
        }

        [Fact]
        public async Task Handle_ProfileNotFound_ThrowsNotFoundException()
        {
            _currentUser.UserId.Returns(Guid.NewGuid());
            _profileReadRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ProfileDto?)null);

            var query = new GetUnreadCountQuery();
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}