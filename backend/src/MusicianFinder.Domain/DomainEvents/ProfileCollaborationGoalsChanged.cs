using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие изменения целей сотрудничества в профиле.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileCollaborationGoalsChanged(Guid ProfileId) : IDomainEvent;
}