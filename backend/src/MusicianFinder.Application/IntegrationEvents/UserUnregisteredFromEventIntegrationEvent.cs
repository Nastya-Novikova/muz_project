namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие отмены регистрации профиля с мероприятия.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record UserUnregisteredFromEventIntegrationEvent(Guid EventId, Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "event.user_unregistered";
        public int Version => 1;
    }
}