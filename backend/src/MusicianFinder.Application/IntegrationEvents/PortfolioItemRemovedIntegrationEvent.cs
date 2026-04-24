namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие удаления элемента портфолио.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    /// <param name="PortfolioItemId">Идентификатор удалённого элемента.</param>
    public sealed record PortfolioItemRemovedIntegrationEvent(Guid ProfileId, Guid PortfolioItemId) : IIntegrationEvent
    {
        public string EventName => "portfolio.item_removed";
        public int Version => 1;
    }
}