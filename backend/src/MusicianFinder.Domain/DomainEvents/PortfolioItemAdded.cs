using MusicianFinder.SharedKernel;

namespace MusicianFinder.Domain.DomainEvents
{
    /// <summary>
    /// Событие добавления элемента портфолио.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    /// <param name="PortfolioItemId">Идентификатор добавленного элемента портфолио.</param>
    public sealed record PortfolioItemAdded(Guid ProfileId, Guid PortfolioItemId) : IDomainEvent;
}