using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие обновления основной информации профиля (имя, город, возраст, описание).
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileCoreInfoUpdated(Guid ProfileId) : IDomainEvent;
}