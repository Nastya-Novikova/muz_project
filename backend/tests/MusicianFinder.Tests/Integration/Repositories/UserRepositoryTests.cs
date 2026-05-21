using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Repositories
{
    public class UserRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private UserRepository _repository = null!;

        public UserRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new UserRepository(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Add_WhenUserIsValid_SavesToDatabase()
        {
            LogInfo("Test: Add user");
            var user = new UserBuilder().WithEmail("adduser@test.com").Build();
            _repository.Add(user);
            await _dbContext.SaveChangesAsync();

            var saved = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
            saved.Should().NotBeNull();
            saved!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsUser()
        {
            LogInfo("Test: Get user by ID");
            var user = new UserBuilder().WithEmail("getbyid@test.com").Build();
            _repository.Add(user);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(user.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeleted_ReturnsNull()
        {
            LogInfo("Test: Get deleted user returns null");
            var user = new UserBuilder().WithEmail("deleteduser@test.com").Build();
            _repository.Add(user);
            await _dbContext.SaveChangesAsync();
            user.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(user.Id);
            retrieved.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenExists_ReturnsUser()
        {
            LogInfo("Test: Get user by email");
            var email = "getbyemail@test.com";
            var user = new UserBuilder().WithEmail(email).Build();
            _repository.Add(user);
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByEmailAsync(email);
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenNotExists_ReturnsNull()
        {
            LogInfo("Test: Get user by non-existing email returns null");
            var retrieved = await _repository.GetByEmailAsync("nonexistent@test.com");
            retrieved.Should().BeNull();
        }
    }
}