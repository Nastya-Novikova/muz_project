using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Диспатчер доменных событий, использующий MediatR для публикации событий.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainEventDispatcher"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task DispatchAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            var domainEvents = aggregateRoot.DomainEvents.ToList();
            aggregateRoot.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}