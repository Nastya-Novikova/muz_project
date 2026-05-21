using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Favorites;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Favorites
{
    public class GetFavoritesQueryHandlerTests : TestBase
    {
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;
        private readonly GetFavoritesQueryHandler _handler;

        public GetFavoritesQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _favoriteReadRepository = Substitute.For<IFavoriteReadRepository>();
            _currentUser = Substitute.For<ICurrentUserService>();
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _suggestionReadRepository = Substitute.For<ICollaborationSuggestionReadRepository>();
            _handler = new GetFavoritesQueryHandler(_favoriteReadRepository, _currentUser, _profileReadRepository, _suggestionReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsFavoritesWithPagination()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUser.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);
            var favProfileId = Guid.NewGuid();
            var paged = new PagedResult<ProfileDto>
            {
                Items = new List<ProfileDto> { new ProfileDto { Id = favProfileId } },
                Total = 1
            };
            _favoriteReadRepository.GetFavoritesAsync(profile.Id, 1, 20, Arg.Any<CancellationToken>()).Returns(paged);
            _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(profile.Id, Arg.Is<List<Guid>>(l => l.Contains(favProfileId)), Arg.Any<CancellationToken>())
                .Returns(new HashSet<Guid>());

            var query = new GetFavoritesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Items[0].IsFavorite.Should().BeTrue();
        }
    }
}