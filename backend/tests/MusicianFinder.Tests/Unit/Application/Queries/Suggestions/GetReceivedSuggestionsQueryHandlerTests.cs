using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Suggestions;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Suggestions
{
    public class GetReceivedSuggestionsQueryHandlerTests : TestBase
    {
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetReceivedSuggestionsQueryHandler _handler;

        public GetReceivedSuggestionsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _suggestionReadRepository = Substitute.For<ICollaborationSuggestionReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetReceivedSuggestionsQueryHandler(_suggestionReadRepository, _currentUser, _profileReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsReceivedSuggestions()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUser.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);
            var paged = new PagedResult<SuggestionDto>
            {
                Items = new List<SuggestionDto> { new SuggestionDto() },
                Total = 1
            };
            _suggestionReadRepository.GetReceivedAsync(profile.Id, 1, 20, Arg.Any<CancellationToken>()).Returns(paged);

            var query = new GetReceivedSuggestionsQuery { Page = 1, Limit = 20 };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(paged);
        }
    }
}