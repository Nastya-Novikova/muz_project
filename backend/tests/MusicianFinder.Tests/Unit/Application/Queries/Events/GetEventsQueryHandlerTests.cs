using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Events
{
    public class GetEventsQueryHandlerTests : TestBase
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetEventsQueryHandler _handler;

        public GetEventsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _eventReadRepository = Substitute.For<IEventReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetEventsQueryHandler(_eventReadRepository, _currentUserService, _profileReadRepository);
        }

        [Fact]
        public async Task Handle_Unauthenticated_ReturnsEventsWithoutFlags()
        {
            var pagedResult = new PagedResult<EventDto> { Items = new List<EventDto> { new EventDto { Id = Guid.NewGuid() } }, Total = 1 };
            _eventReadRepository.SearchAsync(Arg.Any<EventFilterDto>(), Arg.Any<CancellationToken>()).Returns(pagedResult);
            _currentUserService.IsAuthenticated.Returns(false);

            var query = new GetEventsQuery { Page = 1, Limit = 10 };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Items[0].IsRegistered.Should().BeFalse();
            result.Items[0].IsCreator.Should().BeFalse();
        }
    }
}