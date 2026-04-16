using backend.Models.Classes;

namespace backend.Models.Classes;

/// <summary>
/// Аудиозапись в портфолио
/// </summary>
public class PortfolioAudio
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
    /// Аудиофайл (бинарные данные)
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    public string MimeType { get; set; } = "audio/mpeg";

    /// <summary>
    /// Длительность в секундах
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство
    public MusicianProfile Profile { get; set; } = null!;
}