namespace backend.Models.Classes;

/// <summary>
/// Предложение о сотрудничестве
/// </summary>
public class CollaborationSuggestion
{
    /// <summary>
    /// Идентификатор предложения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID отправителя
    /// </summary>
    public Guid FromProfileId { get; set; }

    /// <summary>
    /// ID получателя
    /// </summary>
    public Guid ToProfileId { get; set; }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Статус: pending, accepted, rejected, withdrawn
    /// </summary>
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
    //[ForeignKey("FromProfileId")]
    public MusicianProfile FromProfile { get; set; } = null!;

    public MusicianProfile ToProfile { get; set; } = null!;
}