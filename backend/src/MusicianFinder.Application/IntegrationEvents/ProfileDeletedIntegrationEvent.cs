namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие удаления профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор удалённого профиля.</param>
    public sealed record ProfileDeletedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.deleted";
        public int Version => 1;
    }
}