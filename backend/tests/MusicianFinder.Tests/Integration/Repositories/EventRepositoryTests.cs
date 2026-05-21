using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Infrastructure.Persistence.Repositories;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Builders;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Repositories
{
    public class EventRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private EventRepository _repository = null!;

        public EventRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new EventRepository(_dbContext);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Add_WhenEventIsValid_SavesToDatabase()
        {
            LogInfo("Test: Add event");
            var creator = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _repository.Add(ev);
            await _dbContext.SaveChangesAsync();

            var saved = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == ev.Id);
            saved.Should().NotBeNull();
            saved!.Title.Value.Should().Be(ev.Title.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEventExists_ReturnsEventWithRegistrations()
        {
            LogInfo("Test: Get event by ID with registrations");
            var creator = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _repository.Add(ev);
            await _dbContext.SaveChangesAsync();

            var registrant = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(registrant);
            await _dbContext.SaveChangesAsync();

            await _repository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id,
                e => e.Register(registrant.Id));
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(ev.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Registrations.Should().HaveCount(1);
            retrieved.Registrations.First().ProfileId.Should().Be(registrant.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeleted_ReturnsNull()
        {
            LogInfo("Test: Get deleted event returns null");
            var creator = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _repository.Add(ev);
            await _dbContext.SaveChangesAsync();

            ev.MarkAsDeleted();
            await _dbContext.SaveChangesAsync();

            var retrieved = await _repository.GetByIdAsync(ev.Id);
            retrieved.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_ForRegistration_TracksNewEntity()
        {
            LogInfo("Test: ExecuteAndTrackNewOwnedAsync for EventRegistration");
            var creator = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).Build();
            _repository.Add(ev);
            await _dbContext.SaveChangesAsync();

            var registrant = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(registrant);
            await _dbContext.SaveChangesAsync();

            await _repository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                ev.Id,
                e => e.Register(registrant.Id));
            await _dbContext.SaveChangesAsync();

            var updatedEvent = await _repository.GetByIdAsync(ev.Id);
            updatedEvent!.Registrations.Should().Contain(r => r.ProfileId == registrant.Id);
        }

        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_WhenEventNotFound_Throws()
        {
            LogInfo("Test: ExecuteAndTrackNewOwnedAsync with non-existent event throws");
            var registrant = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(registrant);
            await _dbContext.SaveChangesAsync();

            Func<Task> act = async () => await _repository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(
                Guid.NewGuid(),
                e => e.Register(registrant.Id));

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // ---------------------- NEW TEST ----------------------
        [Fact]
        public async Task ExecuteAndTrackNewOwnedAsync_ForEventRegistration_WhenEventFull_Throws()
        {
            var creator = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(creator);
            await _dbContext.SaveChangesAsync();

            var ev = new EventBuilder().WithCreatorProfileId(creator.Id).WithMaxParticipants(1).Build();
            _repository.Add(ev);
            await _dbContext.SaveChangesAsync();

            var registrant1 = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(registrant1);
            await _dbContext.SaveChangesAsync();
            await _repository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(ev.Id, e => e.Register(registrant1.Id));
            await _dbContext.SaveChangesAsync();

            var registrant2 = new MusicianProfileBuilder().Build();
            _dbContext.MusicianProfiles.Add(registrant2);
            await _dbContext.SaveChangesAsync();

            Func<Task> act = async () => await _repository.ExecuteAndTrackNewOwnedAsync<EventRegistration>(ev.Id, e => e.Register(registrant2.Id));
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}