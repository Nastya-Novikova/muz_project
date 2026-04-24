using System;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Регистрация пользователя на мероприятие.
    /// </summary>
    public class EventRegistration
    {
        private EventRegistration()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр регистрации.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
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

        /// <summary>
        /// Мероприятие (навигационное свойство).
        /// </summary>
        public Event? Event { get; private set; }

        /// <summary>
        /// Профиль участника (навигационное свойство).
        /// </summary>
        public MusicianProfile? Profile { get; private set; }
    }
}