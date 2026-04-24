namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие отклонения предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    public sealed record CollaborationSuggestionRejectedIntegrationEvent(Guid SuggestionId) : IIntegrationEvent
    {
        public string EventName => "collaboration.suggestion_rejected";
        public int Version => 1;
    }
}