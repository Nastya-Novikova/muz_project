using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие регистрации пользователя на мероприятие.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    /// <param name="ProfileId">Идентификатор профиля зарегистрировавшегося участника.</param>
    public sealed record UserRegisteredToEvent(Guid EventId, Guid ProfileId) : IDomainEvent;
}