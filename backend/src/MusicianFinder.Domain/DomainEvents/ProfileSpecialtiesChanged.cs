using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие изменения набора специальностей в профиле.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileSpecialtiesChanged(Guid ProfileId) : IDomainEvent;
}