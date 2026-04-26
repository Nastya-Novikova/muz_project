using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие добавления профиля в избранное.
    /// </summary>
    /// <param name="AddedByProfileId">Идентификатор профиля, добавившего в избранное.</param>
    /// <param name="TargetProfileId">Идентификатор профиля, добавленного в избранное.</param>
    public sealed record FavoriteAdded(Guid AddedByProfileId, Guid TargetProfileId) : IDomainEvent;
}