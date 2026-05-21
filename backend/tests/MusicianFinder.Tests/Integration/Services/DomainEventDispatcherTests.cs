using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using MusicianFinder.Application;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class DomainEventDispatcherTests : TestBase
    {
        private readonly IMediator _mediator;
        private readonly DomainEventDispatcher _dispatcher;

        public DomainEventDispatcherTests(ITestOutputHelper output) : base(output)
        {
            _mediator = Substitute.For<IMediator>();
            _dispatcher = new DomainEventDispatcher(_mediator);
        }

        [Fact]
        public async Task DispatchAsync_AggregateWithEvents_PublishesEachEvent()
        {
            var aggregate = new TestableAggregate();
            aggregate.AddTestEvent(new TestDomainEvent());
            aggregate.AddTestEvent(new TestDomainEvent());

            await _dispatcher.DispatchAsync(aggregate, CancellationToken.None);

            // Проверяем, что Publish был вызван 2 раза с DomainEventNotification<TestDomainEvent>
            await _mediator.Received(2).Publish(
                Arg.Any<DomainEventNotification<TestDomainEvent>>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DispatchAsync_AggregateWithoutEvents_DoesNothing()
        {
            var aggregate = new TestableAggregate();

            await _dispatcher.DispatchAsync(aggregate, CancellationToken.None);

            await _mediator.DidNotReceive().Publish(
                Arg.Any<DomainEventNotification<TestDomainEvent>>(),
                Arg.Any<CancellationToken>());
        }
    }

    public class TestableAggregate : AggregateRoot
    {
        public void AddTestEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
    }

    public record TestDomainEvent : IDomainEvent { }
}