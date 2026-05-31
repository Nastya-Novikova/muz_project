using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.ReadRepositories
{
    public class FavoriteReadRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private IFavoriteReadRepository _repository = null!;

        public FavoriteReadRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            var refDataRepo = new ReferenceDataReadRepository(_dbContext, _fixture.Mapper);
            _repository = new FavoriteReadRepository(_dbContext, _fixture.Mapper, refDataRepo);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetFavoritesAsync_WhenFavoritesExist_ReturnsPagedResult()
        {
            var user = new UserBuilder().WithEmail("favuser@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Создаём профили, но не сохраняем их пока
            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            var favorite = new MusicianProfileBuilder().Build();

            // Добавляем избранное на ещё не сохранённом профиле
            profile.AddToFavorites(favorite.Id);

            // Сохраняем оба профиля вместе с owned-связью одним махом
            _dbContext.MusicianProfiles.AddRange(profile, favorite);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetFavoritesAsync(profile.Id, 1, 10, CancellationToken.None);
            result.Total.Should().Be(1);
            result.Items[0].Id.Should().Be(favorite.Id);
        }

        [Fact]
        public async Task GetFavoritedProfileIdsAsync_ReturnsCorrectIds()
        {
            var user = new UserBuilder().WithEmail("favids@test.com").Build();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new MusicianProfileBuilder().WithUserId(user.Id).Build();
            var target1 = new MusicianProfileBuilder().Build();
            var target2 = new MusicianProfileBuilder().Build();

            profile.AddToFavorites(target1.Id);

            _dbContext.MusicianProfiles.AddRange(profile, target1, target2);
            await _dbContext.SaveChangesAsync();

            var ids = new[] { target1.Id, target2.Id };
            var result = await _repository.GetFavoritedProfileIdsAsync(profile.Id, ids, CancellationToken.None);
            result.Should().Contain(target1.Id);
            result.Should().NotContain(target2.Id);
        }
    }
}