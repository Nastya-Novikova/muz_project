using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие отмены регистрации пользователя с мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    /// <param name="ProfileId">Идентификатор профиля, отменившего регистрацию.</param>
    public sealed record UserUnregisteredFromEvent(Guid EventId, Guid ProfileId) : IDomainEvent;
}