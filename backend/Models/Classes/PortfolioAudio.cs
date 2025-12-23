using backend.Models.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Аудиозапись в портфолио
/// </summary>
[Table("PortfolioAudio")]
public class PortfolioAudio
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ID профиля владельца
    /// </summary>
    [Required]
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Аудиофайл (бинарные данные)
    /// </summary>
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    [MaxLength(50)]
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
    [ForeignKey("ProfileId")]
    public MusicianProfile Profile { get; set; } = null!;
}