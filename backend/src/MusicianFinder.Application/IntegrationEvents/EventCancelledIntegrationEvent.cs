namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие отмены мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventCancelledIntegrationEvent(Guid EventId) : IIntegrationEvent
    {
        public string EventName => "event.cancelled";
        public int Version => 1;
    }
}