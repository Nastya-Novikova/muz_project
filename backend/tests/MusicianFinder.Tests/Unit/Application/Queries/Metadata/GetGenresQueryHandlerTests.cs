using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Metadata;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Metadata
{
    public class GetGenresQueryHandlerTests : TestBase
    {
        private readonly IReferenceDataReadRepository _referenceRepository;
        private readonly GetGenresQueryHandler _handler;

        public GetGenresQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _referenceRepository = Substitute.For<IReferenceDataReadRepository>();
            _handler = new GetGenresQueryHandler(_referenceRepository);
        }

        [Fact]
        public async Task Handle_ReturnsAllGenres()
        {
            var genres = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Rock" } };
            _referenceRepository.GetGenresAsync(Arg.Any<CancellationToken>()).Returns(genres);

            var query = new GetGenresQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEquivalentTo(genres);
        }
    }
}