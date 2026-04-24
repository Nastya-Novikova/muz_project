namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие принятия предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    public sealed record CollaborationSuggestionAcceptedIntegrationEvent(Guid SuggestionId) : IIntegrationEvent
    {
        public string EventName => "collaboration.suggestion_accepted";
        public int Version => 1;
    }
}
