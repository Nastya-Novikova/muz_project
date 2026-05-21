using System;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.DTOs.Events;
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
    public class EventReadRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private IEventReadRepository _repository = null!;

        public EventReadRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            var refDataRepo = new ReferenceDataReadRepository(_dbContext, _fixture.Mapper);
            _repository = new EventReadRepository(_dbContext, _fixture.Mapper, refDataRepo);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetByIdAsync_WhenEventExists_ReturnsEventDto()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).WithTitle("Test Event").Build();
            _dbContext.Events.Add(ev);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(ev.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(ev.Id);
            result.Title.Should().Be("Test Event");
            result.CreatorFullName.Should().Be(creator.FullName.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeleted_ReturnsNull()
        {
            var ev = new EventBuilder().Build();
            _dbContext.Events.Add(ev);
            await _dbContext.SaveChangesAsync();
            ev.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(ev.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task SearchAsync_WithCityFilter_ReturnsFilteredEvents()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_city_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev1 = new EventBuilder().WithCreatorProfileId(creator.Id).WithCityId(1).Build();
            var ev2 = new EventBuilder().WithCreatorProfileId(creator.Id).WithCityId(2).Build();
            _dbContext.Events.AddRange(ev1, ev2);
            await _dbContext.SaveChangesAsync();

            var filter = new EventFilterDto { CityId = 1, Page = 1, Limit = 10 };
            var result = await _repository.SearchAsync(filter);
            result.Total.Should().Be(1);
            result.Items[0].Id.Should().Be(ev1.Id);
        }

        [Fact]
        public async Task GetCreatedEventsAsync_ReturnsEventsByCreator()
        {
            var creator1 = new MusicianProfileBuilder().WithEmail($"creator1_{Guid.NewGuid()}@test.com").Build();
            var creator2 = new MusicianProfileBuilder().WithEmail($"creator2_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.AddRange(creator1, creator2);
            await _dbContext.SaveChangesAsync();

            var ev1 = new EventBuilder().WithCreatorProfileId(creator1.Id).Build();
            var ev2 = new EventBuilder().WithCreatorProfileId(creator1.Id).Build();
            var ev3 = new EventBuilder().WithCreatorProfileId(creator2.Id).Build();
            _dbContext.Events.AddRange(ev1, ev2, ev3);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetCreatedEventsAsync(creator1.Id, 1, 10);
            result.Total.Should().Be(2);
            result.Items.Should().Contain(e => e.Id == ev1.Id);
            result.Items.Should().Contain(e => e.Id == ev2.Id);
        }

        [Fact]
        public async Task GetRegisteredEventsAsync_ReturnsEventsUserRegistered()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_reg_{Guid.NewGuid()}@test.com").Build();
            var registrant = new MusicianProfileBuilder().WithEmail($"registrant_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.AddRange(creator, registrant);
            await _dbContext.SaveChangesAsync();

            var ev1 = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            var ev2 = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            ev1.Register(registrant.Id);
            _dbContext.Events.AddRange(ev1, ev2);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetRegisteredEventsAsync(registrant.Id, 1, 10);
            result.Total.Should().Be(1);
            result.Items[0].Id.Should().Be(ev1.Id);
        }

        [Fact]
        public async Task IsProfileRegisteredAsync_WhenRegistered_ReturnsTrue()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_isreg_{Guid.NewGuid()}@test.com").Build();
            var registrant = new MusicianProfileBuilder().WithEmail($"registrant_isreg_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.AddRange(creator, registrant);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            ev.Register(registrant.Id);
            _dbContext.Events.Add(ev);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.IsProfileRegisteredAsync(ev.Id, registrant.Id);
            result.Should().BeTrue();

            var notRegistered = await _repository.IsProfileRegisteredAsync(ev.Id, creator.Id);
            notRegistered.Should().BeFalse();
        }

        [Fact]
        public async Task SearchAsync_WithDateFilter_ReturnsFilteredEvents()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_date_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var futureEvent1 = new EventBuilder()
                .WithCreatorProfileId(creator.Id)
                .WithStartDateTime(nowUnspecified.AddDays(1))
                .WithTitle("Future1")
                .Build();
            var futureEvent2 = new EventBuilder()
                .WithCreatorProfileId(creator.Id)
                .WithStartDateTime(nowUnspecified.AddDays(3))
                .WithTitle("Future2")
                .Build();

            _dbContext.Events.AddRange(futureEvent1, futureEvent2);
            await _dbContext.SaveChangesAsync();

            // фильтр: старт позже чем сейчас + 2 дня
            var filter = new EventFilterDto { FromDate = nowUnspecified.AddDays(2), Page = 1, Limit = 10 };
            var result = await _repository.SearchAsync(filter);
            result.Total.Should().Be(1);
            result.Items[0].Id.Should().Be(futureEvent2.Id);
        }

        [Fact(Skip = "IsCreator flag is not set by repository method directly; it's set in query handler")]
        public async Task GetCreatedEventsAsync_IncludesIsCreatorFlag()
        {
            var creator = new MusicianProfileBuilder().WithEmail($"creator_flag_{Guid.NewGuid()}@test.com").Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();
            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _dbContext.Events.Add(ev);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetCreatedEventsAsync(creator.Id, 1, 10);
            result.Items.Should().ContainSingle().Which.IsCreator.Should().BeTrue();
        }
    }
}