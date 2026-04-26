namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Регистрация профиля музыканта на мероприятие. Является частью агрегата Event.
    /// </summary>
    public class EventRegistration
    {
        private EventRegistration() { }

        /// <summary>
        /// Инициализирует новый экземпляр регистрации.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="profileId">Идентификатор регистрируемого профиля.</param>
        public EventRegistration(Guid eventId, Guid profileId)
        {
            EventId = eventId;
            ProfileId = profileId;
            RegisteredAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// Идентификатор профиля участника.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Дата и время регистрации.
        /// </summary>
        public DateTime RegisteredAt { get; private set; }
    }
}