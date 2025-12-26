using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Профиль музыканта
/// </summary>
[Table("MusicianProfiles")]
public class MusicianProfile : ISoftDeletable
{
    /// <summary>
    /// Идентификатор профиля
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /*/// <summary>
    /// ID пользователя
    /// </summary>
    [Required]
    public Guid UserId { get; set; }*/

    /// <summary>
    /// Полное имя
    /// </summary>
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Аватар профиля (бинарные данные)
    /// </summary>
    public byte[]? Avatar { get; set; }

    /// <summary>
    /// Возраст (0-100)
    /// </summary>
    [Range(0, 100)]
    public int? Age { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Phone]
    public string? Phone { get; set; }

    /// <summary>
    /// Telegram
    /// </summary>
    [MaxLength(50)]
    public string? Telegram { get; set; }

    /// <summary>
    /// ID города
    /// </summary>
    [Required]
    public int CityId { get; set; }

    /// <summary>
    /// Опыт (в годах)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Experience { get; set; } = 0;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // === Навигационные свойства ===
    [ForeignKey("CityId")]
    public City City { get; set; } = null!;

    // === Коллекции ===
    public List<Genre> Genres { get; set; } = new();
    public List<MusicalSpecialty> Specialties { get; set; } = new();
    public List<CollaborationGoal> CollaborationGoals { get; set; } = new();

    // === Портфолио ===
    /// <summary>
    /// Аудиозаписи в портфолио
    /// </summary>
    public List<PortfolioAudio> AudioFiles { get; set; } = new();

    /// <summary>
    /// Видеозаписи в портфолио
    /// </summary>
    public List<PortfolioVideo> VideoFiles { get; set; } = new();

    /// <summary>
    /// Фотографии в портфолио
    /// </summary>
    public List<PortfolioPhoto> Photos { get; set; } = new();

    // === Soft-delete ===
    /// <inheritdoc />
    public bool IsDeleted { get; set; } = false;

    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }
}