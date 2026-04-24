namespace MusicianFinder.Application.DTOs.Notifications
{
    /// <summary>
    /// DTO уведомления.
    /// </summary>
    public class NotificationDto
    {
        /// <summary>Идентификатор.</summary>
        public Guid Id { get; set; }
        /// <summary>Тип уведомления.</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>Заголовок.</summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>Сообщение.</summary>
        public string? Message { get; set; }
        /// <summary>URL изображения.</summary>
        public string? ImageUrl { get; set; }
        /// <summary>Тип связанной сущности.</summary>
        public string EntityType { get; set; } = string.Empty;
        /// <summary>Идентификатор связанной сущности.</summary>
        public Guid EntityId { get; set; }
        /// <summary>Прочитано ли.</summary>
        public bool IsRead { get; set; }
        /// <summary>Дата создания.</summary>
        public DateTime CreatedAt { get; set; }
    }
}