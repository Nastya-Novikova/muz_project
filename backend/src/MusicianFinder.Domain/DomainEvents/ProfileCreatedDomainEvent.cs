using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие создания профиля музыканта.
    /// </summary>
    public sealed record ProfileCreatedDomainEvent(Guid ProfileId, Guid UserId) : IDomainEvent;
}