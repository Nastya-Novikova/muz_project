using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Repositories
{
    public class CollaborationSuggestionRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private CollaborationSuggestionRepository _repository = null!;

        public CollaborationSuggestionRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new CollaborationSuggestionRepository(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Add_WhenSuggestionValid_SavesToDatabase()
        {
            LogInfo("Test: Add collaboration suggestion");
            var fromProfile = new MusicianProfileBuilder().Build();
            var toProfile = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.AddRange(fromProfile, toProfile);
            await _dbContext.SaveChangesAsync();

            var suggestion = new CollaborationSuggestionBuilder()
                .FromProfile(fromProfile.Id)
                .ToProfile(toProfile.Id)
                .WithMessage("Test message")
                .Build();

            _repository.Add(suggestion);
            await _dbContext.SaveChangesAsync();

            var saved = await _dbContext.CollaborationSuggestions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == suggestion.Id);
            saved.Should().NotBeNull();
            saved!.FromProfileId.Should().Be(fromProfile.Id);
            saved.ToProfileId.Should().Be(toProfile.Id);
            saved.Message.Should().Be("Test message");
            saved.Status.Should().Be(SuggestionStatus.Pending);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsSuggestion()
        {
            LogInfo("Test: Get collaboration suggestion by ID");
            var fromProfile = new MusicianProfileBuilder().Build();
            var toProfile = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.AddRange(fromProfile, toProfile);
            await _dbContext.SaveChangesAsync();

            var suggestion = new CollaborationSuggestionBuilder()
                .FromProfile(fromProfile.Id)
                .ToProfile(toProfile.Id)
                .Build();
            _repository.Add(suggestion);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(suggestion.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(suggestion.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            LogInfo("Test: Get non-existent suggestion returns null");
            var retrieved = await _repository.GetByIdAsync(System.Guid.NewGuid());
            retrieved.Should().BeNull();
        }

        [Fact(Skip = "GetByProfileIdsAsync not implemented in CollaborationSuggestionRepository")]
        public async Task GetByProfileIdsAsync_ReturnsPendingSuggestions()
        {
            var fromProfile = new MusicianProfileBuilder().Build();
            var toProfile = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.AddRange(fromProfile, toProfile);
            await _dbContext.SaveChangesAsync();

            var suggestion = new CollaborationSuggestionBuilder()
                .FromProfile(fromProfile.Id)
                .ToProfile(toProfile.Id)
                .Build();
            _repository.Add(suggestion);
            await _dbContext.SaveChangesAsync();

            //var result = await _repository.GetByProfileIdsAsync(fromProfile.Id, toProfile.Id);
            //result.Should().NotBeEmpty();
            //result.Should().Contain(s => s.Status == SuggestionStatus.Pending);
        }

        [Fact(Skip = "UpdateStatusAsync not implemented in CollaborationSuggestionRepository")]
        public async Task UpdateStatus_WhenExists_Updates()
        {
            var fromProfile = new MusicianProfileBuilder().Build();
            var toProfile = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.AddRange(fromProfile, toProfile);
            await _dbContext.SaveChangesAsync();

            var suggestion = new CollaborationSuggestionBuilder()
                .FromProfile(fromProfile.Id)
                .ToProfile(toProfile.Id)
                .Build();
            _repository.Add(suggestion);
            await _dbContext.SaveChangesAsync();

            //await _repository.UpdateStatusAsync(suggestion.Id, SuggestionStatus.Accepted);
            await _dbContext.SaveChangesAsync();

            var updated = await _repository.GetByIdAsync(suggestion.Id);
            updated!.Status.Should().Be(SuggestionStatus.Accepted);
        }
    }
}