using MusicianFinder.SharedKernel;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Элемент портфолио (аудио, видео, фото). Принадлежит агрегату MusicianProfile.
    /// </summary>
    public class PortfolioItem : Entity
    {
        private PortfolioItem()
        {
            Title = string.Empty;
            FileUrl = string.Empty;
            MimeType = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый элемент портфолио.
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="fileUrl">URL файла.</param>
        /// <param name="mimeType">MIME-тип.</param>
        /// <param name="type">Тип медиа (аудио, видео, фото).</param>
        /// <param name="duration">Длительность в секундах (для аудио/видео).</param>
        public PortfolioItem(string title, string fileUrl, string mimeType, MediaType type, int? duration = null)
        {
            Id = Guid.NewGuid();
            Title = title;
            FileUrl = fileUrl;
            MimeType = mimeType;
            Type = type;
            Duration = duration;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Название элемента.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Описание элемента.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; private set; }

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// Тип медиа.
        /// </summary>
        public MediaType Type { get; private set; }

        /// <summary>
        /// Длительность в секундах.
        /// </summary>
        public int? Duration { get; private set; }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Устанавливает описание элемента портфолио.
        /// </summary>
        /// <param name="description">Новое описание.</param>
        public void SetDescription(string? description)
        {
            Description = description;
        }
    }
}