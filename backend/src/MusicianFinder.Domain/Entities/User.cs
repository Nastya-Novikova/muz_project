using System;
using System.Collections.Generic;
using System.Linq;
using MusicianFinder.Domain.Common;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Пользователь системы. Корень агрегата.
    /// </summary>
    public class User : AggregateRoot, ISoftDeletable
    {
        private readonly List<Favorite> _favorites = [];

        private User()
        {
            Email = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр пользователя.
        /// </summary>
        /// <param name="email">Email пользователя.</param>
        /// <exception cref="DomainException">Выбрасывается, если email пуст.</exception>
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

        /// <summary>
        /// Уникальный идентификатор пользователя.
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
        /// Роль пользователя.
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// Коллекция избранных профилей.
        /// </summary>
        public IReadOnlyCollection<Favorite> Favorites => _favorites.AsReadOnly();

        /// <summary>
        /// Профиль музыканта, связанный с пользователем.
        /// </summary>
        public MusicianProfile? MusicianProfile { get; private set; }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; }

        /// <inheritdoc />
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Отмечает, что профиль музыканта создан, и связывает его с пользователем.
        /// </summary>
        /// <param name="profile">Профиль музыканта.</param>
        /// <exception cref="DomainException">Выбрасывается, если профиль уже создан или передан null.</exception>
        public void MarkProfileAsCreated(MusicianProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (ProfileCreated)
                throw new DomainException("Профиль уже создан.");

            MusicianProfile = profile;
            ProfileCreated = true;
        }

        /// <summary>
        /// Удаляет связь с профилем (используется при мягком удалении профиля).
        /// </summary>
        public void ClearMusicianProfile()
        {
            MusicianProfile = null;
            ProfileCreated = false;
        }

        /// <summary>
        /// Добавляет профиль в избранное.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <exception cref="DomainException">Выбрасывается, если профиль уже в избранном.</exception>
        public void AddFavorite(Guid profileId)
        {
            if (_favorites.Any(f => f.ProfileId == profileId))
                throw new DomainException("Профиль уже в избранном.");

            _favorites.Add(new Favorite(Id, profileId));
        }

        /// <summary>
        /// Удаляет профиль из избранного.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <exception cref="DomainException">Выбрасывается, если профиль не найден в избранном.</exception>
        public void RemoveFavorite(Guid profileId)
        {
            var favorite = _favorites.FirstOrDefault(f => f.ProfileId == profileId)
                ?? throw new DomainException("Профиль не найден в избранном.");

            _favorites.Remove(favorite);
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