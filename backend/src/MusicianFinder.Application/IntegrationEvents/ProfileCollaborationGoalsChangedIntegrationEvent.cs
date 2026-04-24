namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие изменения целей сотрудничества профиля.
    /// </summary>
    /// <param name="ProfileId">Идентификатор профиля.</param>
    public sealed record ProfileCollaborationGoalsChangedIntegrationEvent(Guid ProfileId) : IIntegrationEvent
    {
        public string EventName => "profile.collaboration_goals_changed";
        public int Version => 1;
    }
}