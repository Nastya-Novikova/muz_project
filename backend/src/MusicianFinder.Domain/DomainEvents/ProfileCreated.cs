using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие создания профиля музыканта.
    /// </summary>
    /// <param name="ProfileId">Идентификатор созданного профиля.</param>
    /// <param name="UserId">Идентификатор пользователя, создавшего профиль.</param>
    public sealed record ProfileCreated(Guid ProfileId, Guid UserId) : IDomainEvent;
}