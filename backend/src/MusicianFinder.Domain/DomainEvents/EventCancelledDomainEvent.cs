using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие отмены мероприятия.
    /// </summary>
    public sealed record EventCancelledDomainEvent(Guid EventId) : IDomainEvent;
}