namespace MusicianFinder.API.Contracts.Responses
{
    /// <summary>
    /// Ответ при создании мероприятия.
    /// </summary>
    public class CreatedEventResponse
    {
        /// <summary>
        /// Идентификатор созданного мероприятия.
        /// </summary>
        public Guid Id { get; set; }
    }
}