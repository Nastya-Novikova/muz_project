using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие обновления профиля музыканта.
    /// </summary>
    public sealed record ProfileUpdatedDomainEvent(Guid ProfileId) : IDomainEvent;
}