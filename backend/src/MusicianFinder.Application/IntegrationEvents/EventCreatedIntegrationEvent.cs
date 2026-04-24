namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие создания мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    public sealed record EventCreatedIntegrationEvent(Guid EventId) : IIntegrationEvent
    {
        public string EventName => "event.created";
        public int Version => 1;
    }
}