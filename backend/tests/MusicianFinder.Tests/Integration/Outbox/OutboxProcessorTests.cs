using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Outbox;
using MusicianFinder.Infrastructure.Persistence;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Outbox
{
    public class OutboxProcessorTests : TestBase, IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private ServiceProvider _serviceProvider = null!;
        private AppDbContext _dbContext = null!;
        private IServiceScopeFactory _scopeFactory = null!;

        public OutboxProcessorTests(CustomWebApplicationFactory factory, ITestOutputHelper output) : base(output)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_factory.GetConnectionString()));
            services.AddScoped(_ => Substitute.For<IIntegrationEventTypeRegistry>());
            services.AddScoped(_ => Substitute.For<IExternalBusPublisher>());
            services.AddScoped(_ => Substitute.For<IExternalNotificationSender>());
            _serviceProvider = services.BuildServiceProvider();
            _scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var scope = _serviceProvider.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OutboxMessage\", \"DeadLetter\" CASCADE");
        }

        public Task DisposeAsync()
        {
            _dbContext?.Dispose();
            _serviceProvider?.Dispose();
            return Task.CompletedTask;
        }

        private async Task<OutboxMessage> CreateTestOutboxMessageAsync(string eventName = "test.event", int version = 1)
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventName = eventName,
                Version = version,
                Payload = "{}",
                OccurredAt = DateTime.UtcNow,
                NextAttemptAt = DateTime.UtcNow,
                RetryCount = 0
            };
            _dbContext.Set<OutboxMessage>().Add(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        [Fact(Skip = "OutboxProcessor.ProcessMessagesAsync is private – requires changing to public/internal")]
        public async Task ProcessMessagesAsync_WhenMessageExists_ProcessesAndMarksCompleted()
        {
            LogInfo("Test: OutboxProcessor processes message and marks completed");
            var message = await CreateTestOutboxMessageAsync();
            var registry = _serviceProvider.GetRequiredService<IIntegrationEventTypeRegistry>();
            var publisher = _serviceProvider.GetRequiredService<IExternalBusPublisher>();
            registry.Resolve("test.event", 1).Returns(typeof(TestIntegrationEvent));

            var processor = new OutboxProcessor(_scopeFactory, _serviceProvider.GetRequiredService<ILogger<OutboxProcessor>>());

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            //await processor.ProcessMessagesAsync(cts.Token);

            var processed = await _dbContext.Set<OutboxMessage>().FirstOrDefaultAsync(m => m.Id == message.Id);
            processed!.ProcessedAt.Should().NotBeNull();
            await publisher.Received(1).PublishAsync(Arg.Any<IIntegrationEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact(Skip = "OutboxProcessor.ProcessMessagesAsync is private – requires changing to public/internal")]
        public async Task ProcessMessagesAsync_WhenMessageFails_RetriesAndMovesToDeadLetterAfterMaxAttempts()
        {
            LogInfo("Test: OutboxProcessor retries and moves to dead letter");
            var message = await CreateTestOutboxMessageAsync();
            var registry = _serviceProvider.GetRequiredService<IIntegrationEventTypeRegistry>();
            var publisher = _serviceProvider.GetRequiredService<IExternalBusPublisher>();

            registry.Resolve("test.event", 1).Returns(typeof(TestIntegrationEvent));
            publisher.PublishAsync(Arg.Any<IIntegrationEvent>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new Exception("Network error")));

            var processor = new OutboxProcessor(_scopeFactory, _serviceProvider.GetRequiredService<ILogger<OutboxProcessor>>());

            for (int i = 0; i < 6; i++)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                //await processor.ProcessMessagesAsync(cts.Token);
            }

            var processed = await _dbContext.Set<OutboxMessage>().FirstOrDefaultAsync(m => m.Id == message.Id);
            processed.Should().BeNull();
            var deadLetter = await _dbContext.Set<DeadLetter>().FirstOrDefaultAsync(d => d.OutboxMessageId == message.Id);
            deadLetter.Should().NotBeNull();
            deadLetter!.Error.Should().Contain("Network error");
        }

        private class TestIntegrationEvent : IIntegrationEvent
        {
            public string EventName => "test.event";
            public int Version => 1;
        }
    }
}