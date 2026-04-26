namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие обновления мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventUpdatedIntegrationEvent(Guid EventId) : IIntegrationEvent
    {
        public string EventName => "event.updated";
        public int Version => 1;
    }
}