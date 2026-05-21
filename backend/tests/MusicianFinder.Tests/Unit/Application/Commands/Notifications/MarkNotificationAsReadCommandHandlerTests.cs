using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Commands.Notifications;
using MusicianFinder.Application.Core.Exceptions;
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
    public class MarkNotificationAsReadCommandHandlerTests : TestBase
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly MarkNotificationAsReadCommandHandler _handler;

        public MarkNotificationAsReadCommandHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileRepository = Substitute.For<IMusicianProfileRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _handler = new MarkNotificationAsReadCommandHandler(_profileRepository, _currentUser);
        }

        [Fact]
        public async Task Handle_Valid_UpdatesIsRead()
        {
            var userId = Guid.NewGuid();
            var profile = new MusicianProfileBuilder().WithUserId(userId).Build();
            var notification = new Notification(
                profile.Id,
                NotificationType.EventRegistration,
                "Test",
                EntityType.Event,
                Guid.NewGuid(),
                EntityType.Event.ToString()
            );
            profile.AddNotification(notification);
            _currentUser.UserId.Returns(userId);
            _profileRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);

            var command = new MarkNotificationAsReadCommand { NotificationId = notification.Id };
            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(MediatR.Unit.Value);
            notification.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_NotificationNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var profile = new MusicianProfileBuilder().WithUserId(userId).Build();
            _currentUser.UserId.Returns(userId);
            _profileRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);

            var command = new MarkNotificationAsReadCommand { NotificationId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_NotificationNotBelongingToUser_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var otherProfile = new MusicianProfileBuilder().Build();
            var notification = new Notification(
                otherProfile.Id,
                NotificationType.EventRegistration,
                "Test",
                EntityType.Event,
                Guid.NewGuid(),
                EntityType.Event.ToString()
            );
            otherProfile.AddNotification(notification);
            _currentUser.UserId.Returns(userId);
            _profileRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(new MusicianProfileBuilder().Build());

            var command = new MarkNotificationAsReadCommand { NotificationId = notification.Id };
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}