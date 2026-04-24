using MusicianFinder.Application.DTOs.Metadata;

namespace MusicianFinder.Application.DTOs.Events
{
    /// <summary>
    /// DTO мероприятия.
    /// </summary>
    public class EventDto
    {
        /// <summary>Идентификатор.</summary>
        public Guid Id { get; set; }
        /// <summary>Название.</summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>Описание.</summary>
        public string? Description { get; set; }
        /// <summary>URL изображения.</summary>
        public string? ImageUrl { get; set; }
        /// <summary>Регион.</summary>
        public LookupItemDto Region { get; set; } = new();
        /// <summary>Город.</summary>
        public LookupItemDto City { get; set; } = new();
        /// <summary>Адрес.</summary>
        public string Address { get; set; } = string.Empty;
        /// <summary>Дата начала.</summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>Дата окончания.</summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>Максимальное число участников.</summary>
        public int MaxParticipants { get; set; }
        /// <summary>Текущее число участников.</summary>
        public int CurrentParticipants { get; set; }
        /// <summary>Зарегистрирован ли текущий пользователь.</summary>
        public bool IsRegistered { get; set; }
        /// <summary>Является ли пользователь создателем.</summary>
        public bool IsCreator { get; set; }
        /// <summary>Статус.</summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>Идентификатор создателя.</summary>
        public Guid CreatorProfileId { get; set; }
        /// <summary>Имя создателя.</summary>
        public string CreatorFullName { get; set; } = string.Empty;
        /// <summary>URL аватара создателя.</summary>
        public string? CreatorAvatarUrl { get; set; }
        /// <summary>Дата создания.</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>Дата обновления.</summary>
        public DateTime UpdatedAt { get; set; }
    }
}