using MediatR;
using MusicianFinder.Application;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Диспатчер доменных событий, использующий MediatR для публикации событий внутри процесса.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainEventDispatcher"/>.
        /// </summary>
        /// <param name="mediator">MediatR посредник.</param>
        public DomainEventDispatcher(IMediator mediator) => _mediator = mediator;

        /// <inheritdoc />
        public async Task DispatchAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            var events = aggregateRoot.DomainEvents.ToList();
            aggregateRoot.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent);
                await _mediator.Publish(notification, cancellationToken);
            }
        }
    }
}