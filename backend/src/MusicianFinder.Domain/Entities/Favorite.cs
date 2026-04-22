using System;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Избранный профиль пользователя.
    /// </summary>
    public class Favorite
    {
        private Favorite()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр избранного.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        public Favorite(Guid userId, Guid profileId)
        {
            UserId = userId;
            ProfileId = profileId;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Дата добавления.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Пользователь.
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// Профиль.
        /// </summary>
        public MusicianProfile? Profile { get; private set; }
    }
}