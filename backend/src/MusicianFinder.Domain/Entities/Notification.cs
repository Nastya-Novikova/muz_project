using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Уведомление пользователя.
    /// </summary>
    public class Notification
    {
        private Notification()
        {
            Title = string.Empty;
        }

        /// <summary>
        /// Инициализирует новое уведомление.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля-получателя.</param>
        /// <param name="type">Тип уведомления.</param>
        /// <param name="title">Заголовок.</param>
        /// <param name="entityType">Тип связанной сущности.</param>
        /// <param name="entityId">Идентификатор связанной сущности.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="imageUrl">URL изображения.</param>
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
        /// Идентификатор профиля получателя.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Тип уведомления.
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Текст уведомления.
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// URL изображения.
        /// </summary>
        public string? ImageUrl { get; private set; }

        /// <summary>
        /// Тип связанной сущности.
        /// </summary>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Идентификатор связанной сущности.
        /// </summary>
        public Guid EntityId { get; private set; }

        /// <summary>
        /// Признак прочтения.
        /// </summary>
        public bool IsRead { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Отмечает уведомление как прочитанное.
        /// </summary>
        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}