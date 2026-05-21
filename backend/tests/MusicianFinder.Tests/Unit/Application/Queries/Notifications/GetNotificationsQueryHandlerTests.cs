using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
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
    public class GetNotificationsQueryHandlerTests : TestBase
    {
        private readonly INotificationReadRepository _notificationReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetNotificationsQueryHandler _handler;

        public GetNotificationsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _notificationReadRepository = Substitute.For<INotificationReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetNotificationsQueryHandler(_notificationReadRepository, _currentUser, _profileReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsPagedNotifications()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUser.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);
            var expectedPage = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto> { new NotificationDto() },
                Total = 1
            };
            _notificationReadRepository.GetForProfileAsync(profile.Id, 1, 20, Arg.Any<CancellationToken>())
                .Returns(expectedPage);

            var query = new GetNotificationsQuery { Page = 1, Limit = 20 };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(expectedPage);
        }
    }
}