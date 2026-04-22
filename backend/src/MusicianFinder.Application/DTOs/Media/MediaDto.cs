namespace MusicianFinder.Application.DTOs.Media
{
    /// <summary>
    /// DTO медиа-контента профиля.
    /// </summary>
    public class MediaDto
    {
        /// <summary>
        /// Аудиозаписи.
        /// </summary>
        public List<AudioDto> Audio { get; set; } = new();

        /// <summary>
        /// Видеозаписи.
        /// </summary>
        public List<VideoDto> Video { get; set; } = new();

        /// <summary>
        /// Фотографии.
        /// </summary>
        public List<PhotoDto> Photos { get; set; } = new();
    }

    /// <summary>
    /// DTO аудиозаписи.
    /// </summary>
    public class AudioDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип.
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Длительность в секундах.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO видеозаписи.
    /// </summary>
    public class VideoDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип.
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Длительность в секундах.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO фотографии.
    /// </summary>
    public class PhotoDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип.
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}