using backend.Models.Enums;

namespace backend.Models.Classes
{
    /// <summary>
    /// Уведомление пользователя
    /// </summary>
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid ProfileId { get; set; }
        public MusicianProfile Profile { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }

        public EntityType EntityType { get; set; }
        public Guid EntityId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
