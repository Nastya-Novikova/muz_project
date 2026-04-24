using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие отмены мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventCancelled(Guid EventId) : IDomainEvent;
}