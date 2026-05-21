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
    public class GetCollaborationGoalsQueryHandlerTests : TestBase
    {
        private readonly IReferenceDataReadRepository _referenceRepository;
        private readonly GetCollaborationGoalsQueryHandler _handler;

        public GetCollaborationGoalsQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _referenceRepository = Substitute.For<IReferenceDataReadRepository>();
            _handler = new GetCollaborationGoalsQueryHandler(_referenceRepository);
        }

        [Fact]
        public async Task Handle_ReturnsAllGoals()
        {
            var goals = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Band" } };
            _referenceRepository.GetCollaborationGoalsAsync(Arg.Any<CancellationToken>()).Returns(goals);

            var query = new GetCollaborationGoalsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().BeEquivalentTo(goals);
        }
    }
}