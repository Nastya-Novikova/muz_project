using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Profiles
{
    public class GetMediaQueryHandlerTests : TestBase
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly GetMediaQueryHandler _handler;

        public GetMediaQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _profileReadRepository = Substitute.For<IProfileReadRepository>();
            _handler = new GetMediaQueryHandler(_profileReadRepository);
        }

        [Fact]
        public async Task Handle_ReturnsMedia()
        {
            var profileId = Guid.NewGuid();
            var media = new MediaDto { Audio = new List<AudioDto> { new AudioDto() } };
            _profileReadRepository.GetMediaAsync(profileId, Arg.Any<CancellationToken>()).Returns(media);

            var query = new GetMediaQuery { ProfileId = profileId };
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(media);
        }

        [Fact]
        public async Task Handle_NotFound_ThrowsNotFoundException()
        {
            _profileReadRepository.GetMediaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((MediaDto?)null);

            var query = new GetMediaQuery { ProfileId = Guid.NewGuid() };
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}