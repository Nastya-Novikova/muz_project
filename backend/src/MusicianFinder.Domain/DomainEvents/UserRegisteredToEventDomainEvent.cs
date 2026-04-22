using System;
using MusicianFinder.Domain.Common;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие регистрации пользователя на мероприятие.
    /// </summary>
    public sealed record UserRegisteredToEventDomainEvent(Guid EventId, Guid ProfileId) : IDomainEvent;
}