using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Пользователь системы.
    /// </summary>
    public class User : ISoftDeletable
    {
        private readonly List<Favorite> _favorites = new();

        private User() { } // для EF Core

        public User(string email)
        {
            Id = Guid.NewGuid();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            CreatedAt = DateTime.UtcNow;
            Role = UserRole.User;
            ProfileCreated = false;
            IsDeleted = false;
        }

        /// <summary>
        /// Уникальный идентификатор.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Email пользователя.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Флаг, указывающий, создан ли профиль музыканта.
        /// </summary>
        public bool ProfileCreated { get; private set; }

        /// <summary>
        /// Дата регистрации.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Роль пользователя в системе.
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// Список избранных профилей.
        /// </summary>
        public IReadOnlyCollection<Favorite> Favorites => _favorites.AsReadOnly();

        /// <summary>
        /// Музыкальный профиль пользователя (связь один-к-одному).
        /// </summary>
        public MusicianProfile? MusicianProfile { get; private set; }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        // Методы бизнес-логики

        public void MarkProfileAsCreated(MusicianProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            MusicianProfile = profile;
            ProfileCreated = true;
        }

        public void ClearMusicianProfile()
        {
            MusicianProfile = null;
            ProfileCreated = false;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}
