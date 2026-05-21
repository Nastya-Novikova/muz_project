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
    public class GetCitiesQueryHandlerTests : TestBase
    {
        private readonly IReferenceDataReadRepository _referenceRepository;
        private readonly GetCitiesQueryHandler _handler;

        public GetCitiesQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _referenceRepository = Substitute.For<IReferenceDataReadRepository>();
            _handler = new GetCitiesQueryHandler(_referenceRepository);
        }

        [Fact]
        public async Task Handle_ReturnsAllCities()
        {
            var cities = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Moscow" } };
            _referenceRepository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns(cities);

            var query = new GetCitiesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEquivalentTo(cities);
        }
    }
}