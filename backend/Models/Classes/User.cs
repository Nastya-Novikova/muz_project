using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Пользователь системы
/// </summary>
[Table("Users")]
public class User : ISoftDeletable
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Email пользователя
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя
    /// </summary>
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Аватар пользователя (бинарные данные)
    /// </summary>
    public byte[]? Avatar { get; set; }

    /// <summary>
    /// Флаг завершён ли профиль
    /// </summary>
    public bool ProfileCompleted { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Список ID избранных профилей
    /// </summary>
    public List<Guid> FavoriteProfileIds { get; set; } = new();

    // === Навигационное свойство ===
    /// <summary>
    /// Музыкальный профиль пользователя (связь один-к-одному)
    /// </summary>
    public MusicianProfile? MusicianProfile { get; set; }

    // === Soft-delete ===
    /// <inheritdoc />
    public bool IsDeleted { get; set; } = false;

    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }
}