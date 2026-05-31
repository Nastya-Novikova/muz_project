using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application.Core.Exceptions;
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
    public class GetProfileByIdQueryHandlerTests : TestBase
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;
        private readonly GetProfileByIdQueryHandler _handler;

        public GetProfileByIdQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _favoriteReadRepository = Substitute.For<IFavoriteReadRepository>();
            _suggestionReadRepository = Substitute.For<ICollaborationSuggestionReadRepository>();
            _handler = new GetProfileByIdQueryHandler(_profileReadRepository, _currentUserService, _favoriteReadRepository, _suggestionReadRepository);
        }

        [Fact]
        public async Task Handle_ExistingProfile_ReturnsProfile()
        {
            var profileId = Guid.NewGuid();
            var profile = new ProfileDto { Id = profileId, FullName = "Test" };
            _profileReadRepository.GetByIdAsync(profileId, Arg.Any<CancellationToken>()).Returns(profile);
            _currentUserService.IsAuthenticated.Returns(false);

            var query = new GetProfileByIdQuery { ProfileId = profileId };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(profile);
        }

        [Fact]
        public async Task Handle_NonExistingProfile_ThrowsNotFoundException()
        {
            _profileReadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ProfileDto?)null);
            var query = new GetProfileByIdQuery { ProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithAuthenticatedUser_SetsFlags()
        {
            var profileId = Guid.NewGuid();
            var myProfileId = Guid.NewGuid();
            var profile = new ProfileDto { Id = profileId };
            var myProfile = new ProfileDto { Id = myProfileId };
            _profileReadRepository.GetByIdAsync(profileId, Arg.Any<CancellationToken>()).Returns(profile);
            _currentUserService.IsAuthenticated.Returns(true);
            _currentUserService.UserId.Returns(Guid.NewGuid());
            _profileReadRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(myProfile);
            _favoriteReadRepository.GetFavoritedProfileIdsAsync(myProfileId, Arg.Is<IEnumerable<Guid>>(ids => ids.Contains(profileId)), Arg.Any<CancellationToken>())
                .Returns(new HashSet<Guid> { profileId });
            _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(myProfileId, Arg.Is<IEnumerable<Guid>>(ids => ids.Contains(profileId)), Arg.Any<CancellationToken>())
                .Returns(new HashSet<Guid> { profileId });

            var query = new GetProfileByIdQuery { ProfileId = profileId };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsFavorite.Should().BeTrue();
            result.IsCollaborated.Should().BeTrue();
        }
    }
}