using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие обновления контактных данных профиля (телефон, Telegram).
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileContactsUpdated(Guid ProfileId) : IDomainEvent;
}