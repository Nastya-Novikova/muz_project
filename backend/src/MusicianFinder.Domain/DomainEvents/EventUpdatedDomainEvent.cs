using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие обновления мероприятия.
    /// </summary>
    public sealed record EventUpdatedDomainEvent(Guid EventId) : IDomainEvent;
}