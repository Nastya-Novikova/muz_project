namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие изменения искомых жанров профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileDesiredGenresChangedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.desired_genres_changed";
        public int Version => 1;
    }
}