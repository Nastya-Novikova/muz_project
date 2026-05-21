using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.ReadRepositories
{
    public class ProfileReadRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private IProfileReadRepository _repository = null!;

        public ProfileReadRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            var refDataRepo = new ReferenceDataReadRepository(_dbContext, _fixture.Mapper);
            _repository = new ProfileReadRepository(_dbContext, _fixture.Mapper, refDataRepo);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetByIdAsync_WhenProfileExists_ReturnsProfileDto()
        {
            var profile = new MusicianProfileBuilder()
                .WithEmail($"getbyid_{Guid.NewGuid()}@test.com")
                .WithFullName("Test User")
                .AddGenre(1)
                .AddSpecialty(2)
                .Build();
            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(profile.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(profile.Id);
            result.FullName.Should().Be("Test User");
            result.Email.Should().Contain("@test.com");
            result.Genres.Should().Contain(g => g.Id == 1);
            result.Specialties.Should().Contain(s => s.Id == 2);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeleted_ReturnsNull()
        {
            var profile = new MusicianProfileBuilder()
                .WithEmail($"deleted_{Guid.NewGuid()}@test.com")
                .Build();
            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();
            profile.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(profile.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenProfileExists_ReturnsProfileDto()
        {
            var user = new UserBuilder().WithEmail($"getbyuser_{Guid.NewGuid()}@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder()
                .WithUserId(user.Id)
                .WithFullName("User Profile")
                .WithEmail($"profile_{Guid.NewGuid()}@test.com")
                .Build();
            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByUserIdAsync(user.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(profile.Id);
            result.FullName.Should().Be("User Profile");
        }

        [Fact]
        public async Task SearchAsync_WithFilters_ReturnsFilteredProfiles()
        {
            var profile1 = new MusicianProfileBuilder()
                .WithFullName("Alice")
                .WithCityId(1)
                .WithEmail($"alice_{Guid.NewGuid()}@test.com")
                .Build();
            var profile2 = new MusicianProfileBuilder()
                .WithFullName("Bob")
                .WithCityId(2)
                .WithEmail($"bob_{Guid.NewGuid()}@test.com")
                .Build();
            var profile3 = new MusicianProfileBuilder()
                .WithFullName("Charlie")
                .WithCityId(1)
                .WithEmail($"charlie_{Guid.NewGuid()}@test.com")
                .Build();
            _dbContext.MusicianProfiles.AddRange(profile1, profile2, profile3);
            await _dbContext.SaveChangesAsync();

            var query = new Application.Queries.Profiles.SearchProfilesQuery { CityId = 1, Page = 1, Limit = 10 };
            var result = await _repository.SearchAsync(query);
            result.Total.Should().Be(2);
            result.Items.Should().Contain(p => p.FullName == "Alice");
            result.Items.Should().Contain(p => p.FullName == "Charlie");
            result.Items.Should().NotContain(p => p.FullName == "Bob");
        }

        [Fact]
        public async Task SearchAsync_WithGenreFilter_ReturnsFilteredProfiles()
        {
            var profile1 = new MusicianProfileBuilder()
                .WithFullName("Genre1")
                .AddGenre(1)
                .WithEmail($"genre1_{Guid.NewGuid()}@test.com")
                .Build();
            var profile2 = new MusicianProfileBuilder()
                .WithFullName("Genre2")
                .AddGenre(2)
                .WithEmail($"genre2_{Guid.NewGuid()}@test.com")
                .Build();
            _dbContext.MusicianProfiles.AddRange(profile1, profile2);
            await _dbContext.SaveChangesAsync();

            var query = new Application.Queries.Profiles.SearchProfilesQuery { GenreIds = new System.Collections.Generic.List<int> { 1 }, Page = 1, Limit = 10 };
            var result = await _repository.SearchAsync(query);
            result.Total.Should().Be(1);
            result.Items[0].FullName.Should().Be("Genre1");
        }

        [Fact]
        public async Task GetMediaAsync_ReturnsMediaSections()
        {
            var profile = new MusicianProfileBuilder()
                .WithEmail($"media_{Guid.NewGuid()}@test.com")
                .Build();
            profile.AddPortfolioItem(new PortfolioItem("Song", "http://url/song.mp3", "audio/mpeg", MediaType.Audio));
            profile.AddPortfolioItem(new PortfolioItem("Clip", "http://url/clip.mp4", "video/mp4", MediaType.Video));
            profile.AddPortfolioItem(new PortfolioItem("Pic", "http://url/pic.jpg", "image/jpeg", MediaType.Photo));
            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetMediaAsync(profile.Id);
            result.Should().NotBeNull();
            result!.Audio.Should().HaveCount(1);
            result.Video.Should().HaveCount(1);
            result.Photos.Should().HaveCount(1);
            result.Audio[0].Title.Should().Be("Song");
        }

        [Fact(Skip = "GetByIdAsync with currentProfileId not implemented in IProfileReadRepository")]
        public async Task GetByIdAsync_WhenAuthenticated_SetsIsFavoriteAndIsCollaborated()
        {
            var currentUser = new UserBuilder().WithEmail("current@test.com").Build();
            _dbContext.Users.Add(currentUser);
            await _dbContext.SaveChangesAsync();
            var currentProfile = new MusicianProfileBuilder().WithUserId(currentUser.Id).WithEmail("currentprof@test.com").Build();
            var otherProfile = new MusicianProfileBuilder().WithEmail("other@test.com").Build();
            _dbContext.MusicianProfiles.AddRange(currentProfile, otherProfile);
            await _dbContext.SaveChangesAsync();

            // Добавляем в избранное
            currentProfile.AddToFavorites(otherProfile.Id);
            // Отправляем collaboration предложение
            var suggestion = new CollaborationSuggestion(currentProfile.Id, otherProfile.Id, "Hello");
            _dbContext.CollaborationSuggestions.Add(suggestion);
            await _dbContext.SaveChangesAsync();

            //var result = await _repository.GetByIdAsync(otherProfile.Id, currentProfile.Id);
            //result.Should().NotBeNull();
            //result!.IsFavorite.Should().BeTrue();
            //result.IsCollaborated.Should().BeTrue();
        }

        [Fact]
        public async Task SearchAsync_WithLookingForFilter_ReturnsFilteredProfiles()
        {
            var profile1 = new MusicianProfileBuilder().WithFullName("LookingBand")
                .WithLookingFor(LookingFor.LookingForBand).WithEmail("band@test.com").Build();
            var profile2 = new MusicianProfileBuilder().WithFullName("LookingMusician")
                .WithLookingFor(LookingFor.LookingForMusician).WithEmail("musician@test.com").Build();
            _dbContext.MusicianProfiles.AddRange(profile1, profile2);
            await _dbContext.SaveChangesAsync();

            var query = new Application.Queries.Profiles.SearchProfilesQuery { LookingFor = "LookingForBand", Page = 1, Limit = 10 };
            var result = await _repository.SearchAsync(query);
            result.Total.Should().Be(1);
            result.Items[0].FullName.Should().Be("LookingBand");
        }
    }
}