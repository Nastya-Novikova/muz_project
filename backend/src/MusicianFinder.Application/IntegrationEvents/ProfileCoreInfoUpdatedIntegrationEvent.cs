namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие обновления основной информации профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileCoreInfoUpdatedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.core_info_updated";
        public int Version => 1;
    }
}