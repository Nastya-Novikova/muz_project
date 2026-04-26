using MediatR;
using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application
{
    /// <summary>
    /// Обёртка доменного события для публикации через MediatR.
    /// </summary>
    /// <typeparam name="TDomainEvent">Тип доменного события.</typeparam>
    public class DomainEventNotification<TDomainEvent> : INotification
        where TDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Доменное событие.
        /// </summary>
        public TDomainEvent DomainEvent { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainEventNotification{TDomainEvent}"/>.
        /// </summary>
        /// <param name="domainEvent">Доменное событие.</param>
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}