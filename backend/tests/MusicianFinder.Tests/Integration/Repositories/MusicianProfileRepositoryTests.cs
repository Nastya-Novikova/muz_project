using System;
using System.Linq;
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
    public class MusicianProfileRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private MusicianProfileRepository _repository = null!;

        public MusicianProfileRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new MusicianProfileRepository(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Add_WhenProfileIsValid_SavesToDatabase()
        {
            var profile = new MusicianProfileBuilder()
                .WithEmail($"valid_{Guid.NewGuid()}@test.com")
                .Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            var saved = await _dbContext.MusicianProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == profile.Id);
            saved.Should().NotBeNull();
            saved!.FullName.Value.Should().Be(profile.FullName.Value);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenExists_ReturnsProfileWithCollections()
        {
            LogInfo("Test: Get profile by user ID with collections");
            var user = new UserBuilder().WithEmail("getbyuser@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder()
                .WithUserId(user.Id)
                .AddGenre(1)
                .AddSpecialty(2)
                .Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByUserIdAsync(user.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(profile.Id);
            retrieved.GenreIds.Should().Contain(g => g.Value == 1);
            retrieved.SpecialtyIds.Should().Contain(s => s.Value == 2);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenDeleted_ReturnsNull()
        {
            LogInfo("Test: Get deleted profile by user ID returns null");
            var user = new UserBuilder().WithEmail("deleteduser@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            profile.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByUserIdAsync(user.Id);
            retrieved.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsProfile()
        {
            LogInfo("Test: Get profile by ID");
            var profile = new MusicianProfileBuilder().Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(profile.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(profile.Id);
        }

        [Fact]
        public async Task AddNotificationAsync_AddsNotificationToProfile()
        {
            LogInfo("Test: Add notification to profile");
            var profile = new MusicianProfileBuilder().Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            var notification = new Notification(profile.Id, NotificationType.CollaborationReceived, "Title", EntityType.CollaborationSuggestion, Guid.NewGuid());
            await _repository.AddNotificationAsync(profile.Id, notification);
            await _dbContext.SaveChangesAsync();

            var updatedProfile = await _repository.GetByUserIdAsync(profile.UserId);
            updatedProfile!.Notifications.Should().Contain(n => n.Id == notification.Id);
        }

        [Fact]
        public async Task AddNotificationAsync_WhenProfileNotFound_Throws()
        {
            LogInfo("Test: Add notification to non-existent profile throws");
            var notification = new Notification(Guid.NewGuid(), NotificationType.CollaborationReceived, "Title", EntityType.CollaborationSuggestion, Guid.NewGuid());
            Func<Task> act = async () => await _repository.AddNotificationAsync(Guid.NewGuid(), notification);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_ForFavorite_TracksNewEntity()
        {
            LogInfo("Test: ExecuteAndTrackNewOwnedAsync for Favorite");
            var user = new UserBuilder().WithEmail("trackfavorite@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            _repository.Add(profile);
            var targetProfile = new MusicianProfileBuilder().Build();
            _repository.Add(targetProfile);
            await _dbContext.SaveChangesAsync();

            await _repository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                user.Id,
                p => p.AddToFavorites(targetProfile.Id));
            await _dbContext.SaveChangesAsync();

            var updatedProfile = await _repository.GetByUserIdAsync(user.Id);
            updatedProfile!.Favorites.Should().Contain(f => f.TargetProfileId == targetProfile.Id);
        }

        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_WhenProfileNotFound_Throws()
        {
            LogInfo("Test: ExecuteAndTrackNewOwnedAsync with non-existent user throws");
            Func<Task> act = async () => await _repository.ExecuteAndTrackNewOwnedAsync<Favorite>(
                Guid.NewGuid(),
                p => p.AddToFavorites(Guid.NewGuid()));
            await act.Should().ThrowAsync<Exception>();
        }

        // ---------------------- NEW TESTS ----------------------
        [Fact]
        public async Task GetByUserIdAsync_WhenExists_ReturnsProfileWithPortfolioAndFavoritesAndNotifications()
        {
            var user = new UserBuilder().WithEmail("fullcollections@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder()
                .WithUserId(user.Id)
                .AddGenre(1)
                .Build();
            profile.AddPortfolioItem(new PortfolioItem("Song", "http://url", "audio/mpeg", MediaType.Audio));
            var targetProfile = new MusicianProfileBuilder().Build();
            profile.AddToFavorites(targetProfile.Id);
            profile.AddNotification(new Notification(profile.Id, NotificationType.CollaborationReceived, "Hello", EntityType.CollaborationSuggestion, Guid.NewGuid()));

            _repository.Add(profile);
            _repository.Add(targetProfile);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByUserIdAsync(user.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Portfolio.Should().HaveCount(1);
            retrieved.Favorites.Should().HaveCount(1);
            retrieved.Notifications.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_ForPortfolioItem_TracksNewEntity()
        {
            var user = new UserBuilder().WithEmail("trackportfolio@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            await _repository.ExecuteAndTrackNewOwnedAsync<PortfolioItem>(
                user.Id,
                p => p.AddPortfolioItem(new PortfolioItem("Demo", "http://url", "audio/mpeg", MediaType.Audio)));

            await _dbContext.SaveChangesAsync();

            var updatedProfile = await _repository.GetByUserIdAsync(user.Id);
            updatedProfile!.Portfolio.Should().ContainSingle()
                .Which.Title.Should().Be("Demo");
        }

        [Fact(Skip = "Business rule not implemented: duplicate notification check")]
        public async Task AddNotificationAsync_WhenNotificationAlreadyExists_DoesNotDuplicate()
        {
            var profile = new MusicianProfileBuilder().Build();
            _repository.Add(profile);
            await _dbContext.SaveChangesAsync();

            var notif = new Notification(profile.Id, NotificationType.CollaborationReceived, "DupTitle", EntityType.CollaborationSuggestion, Guid.NewGuid());
            await _repository.AddNotificationAsync(profile.Id, notif);
            await _dbContext.SaveChangesAsync();

            var duplicate = new Notification(profile.Id, NotificationType.CollaborationReceived, "DupTitle", EntityType.CollaborationSuggestion, notif.EntityId);
            await _repository.AddNotificationAsync(profile.Id, duplicate);
            await _dbContext.SaveChangesAsync();

            var updated = await _repository.GetByUserIdAsync(profile.UserId);
            updated!.Notifications.Should().HaveCount(1);
        }
    }
}