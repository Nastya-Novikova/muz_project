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
    public class GetRegionsQueryHandlerTests : TestBase
    {
        private readonly IReferenceDataReadRepository _referenceRepository;
        private readonly GetRegionsQueryHandler _handler;

        public GetRegionsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _referenceRepository = Substitute.For<IReferenceDataReadRepository>();
            _handler = new GetRegionsQueryHandler(_referenceRepository);
        }

        [Fact]
        public async Task Handle_ReturnsAllRegions()
        {
            var regions = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Москва" } };
            _referenceRepository.GetRegionsAsync(Arg.Any<CancellationToken>()).Returns(regions);

            var query = new GetRegionsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEquivalentTo(regions);
        }
    }
}