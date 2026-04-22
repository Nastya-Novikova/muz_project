using System;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Элемент портфолио (аудио, видео, фото). Принадлежит агрегату MusicianProfile.
    /// </summary>
    public class PortfolioItem
    {
        private PortfolioItem()
        {
            Title = string.Empty;
            FileUrl = string.Empty;
            MimeType = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр элемента портфолио.
        /// </summary>
        /// <param name="title">Название.</param>
        /// <param name="fileUrl">URL файла.</param>
        /// <param name="mimeType">MIME-тип.</param>
        /// <param name="type">Тип медиа.</param>
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
        /// Уникальный идентификатор элемента.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Название элемента.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Описание элемента.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; private set; }

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// Тип медиа (аудио, видео, фото).
        /// </summary>
        public MediaType Type { get; private set; }

        /// <summary>
        /// Длительность в секундах (для аудио/видео).
        /// </summary>
        public int? Duration { get; private set; }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime CreatedAt { get; private set; }
    }
}