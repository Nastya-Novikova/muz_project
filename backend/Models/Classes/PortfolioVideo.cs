using backend.Models.Classes;

namespace backend.Models.Classes;

/// <summary>
/// Видеозапись в портфолио
/// </summary>
public class PortfolioVideo
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID профиля владельца
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Видеофайл (бинарные данные)
    /// </summary>
    public string FileUrl { get; set; }

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    public string MimeType { get; set; } = "video/mp4";

    /// <summary>
    /// Продолжительность в секундах
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство
    public MusicianProfile Profile { get; set; } = null!;
}