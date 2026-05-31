using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Events
{
    public class GetEventByIdQueryHandlerTests : TestBase
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetEventByIdQueryHandler _handler;

        public GetEventByIdQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventReadRepository = Substitute.For<IEventReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetEventByIdQueryHandler(_eventReadRepository, _currentUserService, _profileReadRepository);
        }

        [Fact]
        public async Task Handle_ExistingEvent_ReturnsDto()
        {
            var eventId = Guid.NewGuid();
            var dto = new EventDto { Id = eventId };
            _eventReadRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(dto);
            _currentUserService.IsAuthenticated.Returns(false);

            var query = new GetEventByIdQuery { EventId = eventId };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(dto);
        }

        [Fact]
        public async Task Handle_NonExisting_ThrowsNotFoundException()
        {
            _eventReadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((EventDto?)null);
            var query = new GetEventByIdQuery { EventId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithAuthenticatedUser_SetsIsRegisteredAndIsCreator()
        {
            var eventId = Guid.NewGuid();
            var myProfileId = Guid.NewGuid();
            var dto = new EventDto { Id = eventId, CreatorProfileId = myProfileId };
            _eventReadRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(dto);
            _currentUserService.IsAuthenticated.Returns(true);
            _currentUserService.UserId.Returns(Guid.NewGuid());
            var myProfile = new ProfileDto { Id = myProfileId };
            _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, Arg.Any<CancellationToken>()).Returns(myProfile);
            _eventReadRepository.IsProfileRegisteredAsync(eventId, myProfileId, Arg.Any<CancellationToken>()).Returns(true);

            var query = new GetEventByIdQuery { EventId = eventId };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsCreator.Should().BeTrue();
            result.IsRegistered.Should().BeTrue();
        }
    }
}