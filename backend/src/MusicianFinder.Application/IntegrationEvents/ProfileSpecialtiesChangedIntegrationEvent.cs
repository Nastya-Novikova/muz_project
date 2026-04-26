namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие изменения набора специальностей профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileSpecialtiesChangedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.specialties_changed";
        public int Version => 1;
    }
}