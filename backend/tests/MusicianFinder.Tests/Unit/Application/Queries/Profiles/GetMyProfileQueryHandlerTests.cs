using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class GetMyProfileQueryHandlerTests : TestBase
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly GetMyProfileQueryHandler _handler;

        public GetMyProfileQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _handler = new GetMyProfileQueryHandler(_profileReadRepository, _currentUserService);
        }

        [Fact]
        public async Task Handle_ReturnsMyProfile()
        {
            var userId = Guid.NewGuid();
            var profile = new ProfileDto { Id = Guid.NewGuid() };
            _currentUserService.UserId.Returns(userId);
            _profileReadRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(profile);

            var query = new GetMyProfileQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(profile);
            result.IsMyProfile.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ProfileNotFound_ThrowsNotFoundException()
        {
            _currentUserService.UserId.Returns(Guid.NewGuid());
            _profileReadRepository.GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ProfileDto?)null);

            var query = new GetMyProfileQuery();
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}