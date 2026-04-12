//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using backend.Models.Enums;

namespace backend.Models.Classes;

/// <summary>
/// Пользователь системы
/// </summary>
//[Table("Users")]
public class User : ISoftDeletable
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    //[Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Email пользователя
    /// </summary>
    //[Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Флаг завершён ли профиль
    /// </summary>
    public bool ProfileCreated { get; set; } = false;

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Список избранных профилей
    /// </summary>
    public List<Favorite> Favorites { get; set; } = new List<Favorite>();

    //public List<Guid> FavoriteProfileIds { get; set; } = new();

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