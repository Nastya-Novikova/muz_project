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
    public class GetSpecialtiesQueryHandlerTests : TestBase
    {
        private readonly IReferenceDataReadRepository _referenceRepository;
        private readonly GetSpecialtiesQueryHandler _handler;

        public GetSpecialtiesQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _referenceRepository = Substitute.For<IReferenceDataReadRepository>();
            _handler = new GetSpecialtiesQueryHandler(_referenceRepository);
        }

        [Fact]
        public async Task Handle_ReturnsAllSpecialties()
        {
            var specialties = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Guitar" } };
            _referenceRepository.GetSpecialtiesAsync(Arg.Any<CancellationToken>()).Returns(specialties);

            var query = new GetSpecialtiesQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEquivalentTo(specialties);
        }
    }
}