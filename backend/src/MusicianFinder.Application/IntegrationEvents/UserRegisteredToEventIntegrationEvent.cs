namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие регистрации профиля на мероприятие.
    /// </summary>
    /// <param name="EventId">Идентификатор мероприятия.</param>
    /// <param name="ProfileId">Идентификатор зарегистрированного профиля.</param>
    public sealed record UserRegisteredToEventIntegrationEvent(Guid EventId, Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "event.user_registered";
        public int Version => 1;
    }
}