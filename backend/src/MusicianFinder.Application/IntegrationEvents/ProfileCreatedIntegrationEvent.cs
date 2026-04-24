namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие создания профиля музыканта.
    /// </summary>
    /// <param name="ProfileId">Идентификатор созданного профиля.</param>
    public sealed record ProfileCreatedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.created";
        public int Version => 1;
    }
}