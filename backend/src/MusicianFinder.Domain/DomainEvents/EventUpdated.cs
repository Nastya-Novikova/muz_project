using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие обновления мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventUpdated(Guid EventId) : IDomainEvent;
}