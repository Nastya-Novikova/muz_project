using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Связь многие-ко-многим между мероприятием и профилем музыканта (участники).
    /// </summary>
    public class EventRegistration
    {
        private EventRegistration() { }

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

        // Навигационные свойства
        public Event? Event { get; private set; }
        public MusicianProfile? Profile { get; private set; }
    }
}
