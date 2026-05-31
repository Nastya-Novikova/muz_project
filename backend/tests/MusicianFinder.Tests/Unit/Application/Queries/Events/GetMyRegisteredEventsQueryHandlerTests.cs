using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
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
    public class GetMyRegisteredEventsQueryHandlerTests : TestBase
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly GetMyRegisteredEventsQueryHandler _handler;

        public GetMyRegisteredEventsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventReadRepository = Substitute.For<IEventReadRepository>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _handler = new GetMyRegisteredEventsQueryHandler(_eventReadRepository, _profileReadRepository, _currentUser);
        }

        [Fact]
        public async Task Handle_ReturnsEventsUserRegisteredTo()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUser.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);
            var resultPage = new PagedResult<EventDto> { Items = new List<EventDto> { new EventDto() }, Total = 1 };
            _eventReadRepository.GetRegisteredEventsAsync(profile.Id, 1, 20, Arg.Any<CancellationToken>()).Returns(resultPage);

            var query = new GetMyRegisteredEventsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Items.Should().HaveCount(1);
        }
    }
}