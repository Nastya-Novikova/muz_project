using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.IntegrationEvents;
using MusicianFinder.Infrastructure.Outbox;
using MusicianFinder.Infrastructure.Persistence;
using NSubstitute;
using Xunit;

namespace MusicianFinder.Tests.Integration.Services
{
    public class OutboxWriterTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"outbox_test_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task WriteAsync_IntegrationEvent_AddsOutboxMessage()
        {
            using var context = CreateContext();
            var writer = new OutboxWriter(context);

            var integrationEvent = Substitute.For<IIntegrationEvent>();
            integrationEvent.EventName.Returns("test.event");
            integrationEvent.Version.Returns(1);

            await writer.WriteAsync(integrationEvent);
            await context.SaveChangesAsync(); // необходимо сохранить изменения

            var messages = context.Set<OutboxMessage>().ToList();
            messages.Should().HaveCount(1);
            messages[0].EventName.Should().Be("test.event");
        }
    }
}