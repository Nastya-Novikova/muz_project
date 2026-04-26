namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие изменения набора жанров профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileGenresChangedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.genres_changed";
        public int Version => 1;
    }
}