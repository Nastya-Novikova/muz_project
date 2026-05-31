using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class CurrentProfileProviderTests : TestBase
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IMusicianProfileRepository _profileRepo;
        private readonly CurrentProfileProvider _provider;

        public CurrentProfileProviderTests(ITestOutputHelper output) : base(output)
        {
            _currentUser = Substitute.For<ICurrentUserService>();
            _profileRepo = Substitute.For<IMusicianProfileRepository>();
            _provider = new CurrentProfileProvider(_currentUser, _profileRepo);
        }

        [Fact]
        public async Task GetCurrentProfileAsync_WhenAuthenticatedAndProfileExists_ReturnsProfile()
        {
            var userId = Guid.NewGuid();
            var profile = new MusicianProfileBuilder().WithUserId(userId).Build();

            _currentUser.IsAuthenticated.Returns(true);
            _currentUser.UserId.Returns(userId);
            _profileRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
                        .Returns(profile);

            var result = await _provider.GetCurrentProfileAsync();

            result.Should().NotBeNull();
            result.Id.Should().Be(profile.Id);
        }

        [Fact]
        public async Task GetCurrentProfileAsync_WhenAuthenticatedButNoProfile_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _currentUser.IsAuthenticated.Returns(true);
            _currentUser.UserId.Returns(userId);
            _profileRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
                        .Returns((MusicianProfile?)null);

            Func<Task> act = async () => await _provider.GetCurrentProfileAsync();
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetCurrentProfileAsync_WhenNotAuthenticated_ThrowsForbiddenException()
        {
            _currentUser.IsAuthenticated.Returns(false);
            Func<Task> act = async () => await _provider.GetCurrentProfileAsync();
            await act.Should().ThrowAsync<ForbiddenException>();
        }
    }
}