using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие изменения искомых специальностей в профиле.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileDesiredSpecialtiesChanged(Guid ProfileId) : IDomainEvent;
}