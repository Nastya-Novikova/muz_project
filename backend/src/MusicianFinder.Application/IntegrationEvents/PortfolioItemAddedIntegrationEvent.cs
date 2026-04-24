namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие добавления элемента портфолио.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    /// <param name="PortfolioItemId">Идентификатор добавленного элемента.</param>
    public sealed record PortfolioItemAddedIntegrationEvent(Guid ProfileId, Guid PortfolioItemId) : IIntegrationEvent
    {
        public string EventName => "portfolio.item_added";
        public int Version => 1;
    }
}