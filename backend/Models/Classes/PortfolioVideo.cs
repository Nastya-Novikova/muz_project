using backend.Models.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Видеозапись в портфолио
/// </summary>
[Table("PortfolioVideo")]
public class PortfolioVideo
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
    /// Видеофайл (бинарные данные)
    /// </summary>
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    [MaxLength(50)]
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
    [ForeignKey("ProfileId")]
    public MusicianProfile Profile { get; set; } = null!;
}