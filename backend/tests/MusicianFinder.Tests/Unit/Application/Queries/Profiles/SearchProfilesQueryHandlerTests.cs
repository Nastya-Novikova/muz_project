using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Profiles
{
    public class SearchProfilesQueryHandlerTests : TestBase
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;
        private readonly SearchProfilesQueryHandler _handler;

        public SearchProfilesQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _favoriteReadRepository = Substitute.For<IFavoriteReadRepository>();
            _suggestionReadRepository = Substitute.For<ICollaborationSuggestionReadRepository>();
            _handler = new SearchProfilesQueryHandler(_profileReadRepository, _currentUserService, _favoriteReadRepository, _suggestionReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsFilteredProfiles_WithoutOwnProfile()
        {
            var myProfileId = Guid.NewGuid();
            var otherProfileId = Guid.NewGuid();
            var paged = new PagedResult<ProfileDto>
            {
                Items = new List<ProfileDto>
                {
                    new ProfileDto { Id = myProfileId },
                    new ProfileDto { Id = otherProfileId }
                },
                Total = 2
            };
            _profileReadRepository.SearchAsync(Arg.Any<SearchProfilesQuery>(), Arg.Any<CancellationToken>()).Returns(paged);
            _currentUserService.IsAuthenticated.Returns(true);
            _currentUserService.UserId.Returns(Guid.NewGuid());
            var myProfile = new ProfileDto { Id = myProfileId };
            _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, Arg.Any<CancellationToken>()).Returns(myProfile);
            _favoriteReadRepository.GetFavoritedProfileIdsAsync(myProfileId, Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(new HashSet<Guid> { otherProfileId });
            _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(myProfileId, Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(new HashSet<Guid>());

            var query = new SearchProfilesQuery { Page = 1, Limit = 20 };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Items.Should().HaveCount(1);
            result.Items[0].Id.Should().Be(otherProfileId);
            result.Items[0].IsFavorite.Should().BeTrue();
            result.Items[0].IsCollaborated.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Unauthenticated_ReturnsWithoutFlags()
        {
            var paged = new PagedResult<ProfileDto>
            {
                Items = new List<ProfileDto> { new ProfileDto { Id = Guid.NewGuid() } },
                Total = 1
            };
            _profileReadRepository.SearchAsync(Arg.Any<SearchProfilesQuery>(), Arg.Any<CancellationToken>()).Returns(paged);
            _currentUserService.IsAuthenticated.Returns(false);

            var query = new SearchProfilesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Items.Should().HaveCount(1);
        }
    }
}