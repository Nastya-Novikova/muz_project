using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие создания мероприятия.
    /// </summary>
    public sealed record EventCreatedDomainEvent(Guid EventId) : IDomainEvent;
}