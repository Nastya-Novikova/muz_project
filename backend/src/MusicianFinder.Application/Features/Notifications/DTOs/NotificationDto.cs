using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Features.Notifications.DTOs
{
    /// <summary>
    /// DTO уведомления.
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип уведомления.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// URL изображения.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Тип связанной сущности.
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Идентификатор связанной сущности.
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Прочитано ли.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}