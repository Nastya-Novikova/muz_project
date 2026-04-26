using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие мягкого удаления профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileDeleted(Guid ProfileId) : IDomainEvent;
}