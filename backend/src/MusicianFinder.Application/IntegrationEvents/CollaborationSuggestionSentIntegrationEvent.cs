namespace MusicianFinder.Application.IntegrationEvents
{
    /// <summary>
    /// Интеграционное событие отправки предложения о сотрудничестве.
    /// </summary>
    /// <param name="SuggestionId">Идентификатор предложения.</param>
    /// <param name="FromProfileId">Идентификатор отправителя.</param>
    /// <param name="ToProfileId">Идентификатор получателя.</param>
    public sealed record CollaborationSuggestionSentIntegrationEvent(
        Guid SuggestionId, Guid FromProfileId, Guid ToProfileId) : IIntegrationEvent
    {
        public string EventName => "collaboration.suggestion_sent";
        public int Version => 1;
    }
}