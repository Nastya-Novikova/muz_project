using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Notifications;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Commands.Notifications
{
    public class MarkAllNotificationsAsReadCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly MarkAllNotificationsAsReadCommandHandler _handler;

        public MarkAllNotificationsAsReadCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _handler = new MarkAllNotificationsAsReadCommandHandler(_profileRepository, _currentUser);
        }

        [Fact]
        public async Task Handle_MarksAllUnreadAsRead()
        {
            var userId = Guid.NewGuid();
            var profile = new MusicianProfileBuilder().WithUserId(userId).Build();
            var n1 = new Notification(profile.Id, NotificationType.EventRegistration, "m", EntityType.Event, Guid.NewGuid());
            var n2 = new Notification(profile.Id, NotificationType.EventRegistration, "m", EntityType.Event, Guid.NewGuid());
            n1.MarkAsRead();
            profile.AddNotification(n1);
            profile.AddNotification(n2);
            _currentUser.UserId.Returns(userId);
            _profileRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);

            var command = new MarkAllNotificationsAsReadCommand();
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            profile.Notifications.Should().OnlyContain(n => n.IsRead);
        }
    }
}