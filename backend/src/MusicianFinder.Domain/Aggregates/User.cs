using MusicianFinder.SharedKernel;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Пользователь системы. Корень агрегата.
    /// </summary>
    public class User : AggregateRoot, ISoftDeletable
    {
        private User()
        {
            Email = string.Empty;
        }

        /// <summary>
        /// Инициализирует нового пользователя.
        /// </summary>
        /// <param name="email">Email пользователя.</param>
        public User(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email не может быть пустым.");

            Id = Guid.NewGuid();
            Email = email;
            CreatedAt = DateTime.UtcNow;
            Role = UserRole.User;
            ProfileCreated = false;
            IsDeleted = false;
        }

        /// <summary>Email пользователя.</summary>
        public string Email { get; private set; }

        /// <summary>Создан ли профиль музыканта.</summary>
        public bool ProfileCreated { get; private set; }

        /// <summary>Дата регистрации.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Роль пользователя.</summary>
        public UserRole Role { get; private set; }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Помечает профиль музыканта как созданный.
        /// </summary>
        public void MarkProfileAsCreated()
        {
            ProfileCreated = true;
        }

        /// <summary>
        /// Сбрасывает флаг наличия профиля (после удаления профиля).
        /// </summary>
        public void ClearMusicianProfile()
        {
            ProfileCreated = false;
        }

        /// <summary>
        /// Помечает пользователя как удалённого (мягкое удаление).
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        void ISoftDeletable.MarkAsDeleted() => MarkAsDeleted();
    }
}