using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие удаления элемента портфолио.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    /// <param name="PortfolioItemId">Идентификатор удалённого элемента портфолио.</param>
    public sealed record PortfolioItemRemoved(Guid ProfileId, Guid PortfolioItemId) : IDomainEvent;
}