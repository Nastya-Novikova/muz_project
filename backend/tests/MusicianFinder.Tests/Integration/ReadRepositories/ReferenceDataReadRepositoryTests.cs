using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.ReadRepositories
{
    public class ReferenceDataReadRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private IReferenceDataReadRepository _repository = null!;

        public ReferenceDataReadRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            // Для справочников не обязательно очищать базу, но для единообразия делаем
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new ReferenceDataReadRepository(_dbContext, _fixture.Mapper);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetCitiesAsync_ReturnsAllCities()
        {
            LogInfo("Test: Get all cities");
            var result = await _repository.GetCitiesAsync();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetRegionsAsync_ReturnsAllRegions()
        {
            LogInfo("Test: Get all regions");
            var result = await _repository.GetRegionsAsync();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetGenresAsync_ReturnsAllGenres()
        {
            LogInfo("Test: Get all genres");
            var result = await _repository.GetGenresAsync();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(8);
        }

        [Fact]
        public async Task GetSpecialtiesAsync_ReturnsAllSpecialties()
        {
            LogInfo("Test: Get all specialties");
            var result = await _repository.GetSpecialtiesAsync();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetCollaborationGoalsAsync_ReturnsAllGoals()
        {
            LogInfo("Test: Get all collaboration goals");
            var result = await _repository.GetCollaborationGoalsAsync();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(5);
        }
    }
}