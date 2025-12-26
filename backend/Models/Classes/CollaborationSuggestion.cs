using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Предложение о сотрудничестве
/// </summary>
[Table("CollaborationSuggestions")]
public class CollaborationSuggestion
{
    /// <summary>
    /// Идентификатор предложения
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ID отправителя
    /// </summary>
    [Required]
    public Guid FromProfileId { get; set; }

    /// <summary>
    /// ID получателя
    /// </summary>
    [Required]
    public Guid ToProfileId { get; set; }

    /// <summary>
    /// Сообщение
    /// </summary>
    [MaxLength(500)]
    public string? Message { get; set; }

    /// <summary>
    /// Статус: pending, accepted, rejected, withdrawn
    /// </summary>
    [Required]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // === Навигационные свойства ===
    [ForeignKey("FromUserId")]
    public User FromUser { get; set; } = null!;

    [ForeignKey("ToUserId")]
    public User ToUser { get; set; } = null!;
}