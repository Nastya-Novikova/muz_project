using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Уведомление пользователя.
    /// </summary>
    public class Notification
    {
        private Notification() { }

        public Notification(
            Guid profileId,
            NotificationType type,
            string title,
            EntityType entityType,
            Guid entityId,
            string? message = null,
            string? imageUrl = null)
        {
            Id = Guid.NewGuid();
            ProfileId = profileId;
            Type = type;
            Title = title;
            Message = message;
            ImageUrl = imageUrl;
            EntityType = entityType;
            EntityId = entityId;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// ID профиля получателя.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Тип уведомления.
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Заголовок уведомления.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Текст уведомления.
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// URL изображения (если есть).
        /// </summary>
        public string? ImageUrl { get; private set; }

        /// <summary>
        /// Тип сущности, с которой связано уведомление.
        /// </summary>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Идентификатор связанной сущности.
        /// </summary>
        public Guid EntityId { get; private set; }

        /// <summary>
        /// Флаг прочтения.
        /// </summary>
        public bool IsRead { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        // Навигационное свойство
        public MusicianProfile? Profile { get; private set; }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
