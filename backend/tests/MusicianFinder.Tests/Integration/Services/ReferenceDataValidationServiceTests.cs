using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class ReferenceDataValidationServiceTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private ReferenceDataValidationService _service = null!;

        public ReferenceDataValidationServiceTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _service = new ReferenceDataValidationService(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CityExistsAsync_Existing_ReturnsTrue()
        {
            var result = await _service.CityExistsAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CityExistsAsync_NonExisting_ReturnsFalse()
        {
            var result = await _service.CityExistsAsync(999);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RegionExistsAsync_Existing_ReturnsTrue()
        {
            var result = await _service.RegionExistsAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GenreExistsAsync_Existing_ReturnsTrue()
        {
            var result = await _service.GenreExistsAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SpecialtyExistsAsync_Existing_ReturnsTrue()
        {
            var result = await _service.SpecialtyExistsAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CollaborationGoalExistsAsync_Existing_ReturnsTrue()
        {
            var result = await _service.CollaborationGoalExistsAsync(1);
            result.Should().BeTrue();
        }
    }
}