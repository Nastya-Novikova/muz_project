using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;
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
    public class NotificationReadRepositoryTests : TestBase, IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private AppDbContext _dbContext = null!;
        private INotificationReadRepository _repository = null!;

        public NotificationReadRepositoryTests(DatabaseFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _dbContext = _fixture.CreateDbContext();
            _repository = new NotificationReadRepository(_dbContext, _fixture.Mapper);
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetForProfileAsync_ReturnsNotifications()
        {
            // Создаём профиль и добавляем уведомления до сохранения
            var profile = new MusicianProfileBuilder().Build();
            profile.AddNotification(new Notification(profile.Id, NotificationType.CollaborationReceived, "Title1", EntityType.CollaborationSuggestion, Guid.NewGuid()));
            profile.AddNotification(new Notification(profile.Id, NotificationType.EventRegistration, "Title2", EntityType.Event, Guid.NewGuid()));

            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetForProfileAsync(profile.Id, 1, 10, CancellationToken.None);
            result.Total.Should().Be(2);
            result.Items.Should().Contain(n => n.Title == "Title1");
            result.Items.Should().Contain(n => n.Title == "Title2");
        }

        [Fact]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            var profile = new MusicianProfileBuilder().Build();
            var notif1 = new Notification(profile.Id, NotificationType.CollaborationReceived, "Unread", EntityType.CollaborationSuggestion, Guid.NewGuid());
            var notif2 = new Notification(profile.Id, NotificationType.EventRegistration, "Read", EntityType.Event, Guid.NewGuid());
            notif2.MarkAsRead();
            profile.AddNotification(notif1);
            profile.AddNotification(notif2);

            _dbContext.MusicianProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetUnreadCountAsync(profile.Id, CancellationToken.None);
            result.Should().Be(1);
        }
    }
}