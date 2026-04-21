using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Избранный профиль пользователя.
    /// </summary>
    public class Favorite
    {
        private Favorite() { }

        public Favorite(Guid userId, Guid profileId)
        {
            UserId = userId;
            ProfileId = profileId;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор пользователя, добавившего в избранное.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Идентификатор профиля, добавленного в избранное.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Дата добавления.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        // Навигационные свойства
        public User? User { get; private set; }
        public MusicianProfile? Profile { get; private set; }
    }
}
