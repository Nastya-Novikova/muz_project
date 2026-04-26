using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие удаления профиля из избранного.
    /// </summary>
    /// <param name="AddedByProfileId">Идентификатор профиля, удалившего из избранного.</param>
    /// <param name="TargetProfileId">Идентификатор профиля, удалённого из избранного.</param>
    public sealed record FavoriteRemoved(Guid AddedByProfileId, Guid TargetProfileId) : IDomainEvent;
}