namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие обновления контактных данных профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileContactsUpdatedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.contacts_updated";
        public int Version => 1;
    }
}