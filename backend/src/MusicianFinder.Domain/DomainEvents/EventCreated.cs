using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие создания мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventCreated(Guid EventId) : IDomainEvent;
}