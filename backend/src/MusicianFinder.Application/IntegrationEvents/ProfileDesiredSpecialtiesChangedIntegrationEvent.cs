namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие изменения искомых специальностей профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileDesiredSpecialtiesChangedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.desired_specialties_changed";
        public int Version => 1;
    }
}