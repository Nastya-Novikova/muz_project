using backend.Models.Enums;

namespace backend.Models.Classes;

/// <summary>
/// Профиль музыканта
/// </summary>
public class MusicianProfile : ISoftDeletable
{
    /// <summary>
    /// Идентификатор профиля
    /// </summary>
    public Guid Id { get; set; }

    public ProfileType ProfileType { get; set; } = ProfileType.Individual;

    /// <summary>
    /// Полное имя
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Аватар профиля (бинарные данные)
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Возраст (0-100)
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Telegram
    /// </summary>
    public string? Telegram { get; set; }

    /// <summary>
    /// Идентификатор пользователь в Вконтакте
    /// </summary>
    public string? VkUserId { get; set; }

    public bool NotifyByEmail { get; set; } = true;

    public bool NotifyByVk { get; set; }

    /// <summary>
    /// ID города
    /// </summary>
    public int CityId { get; set; }

    /// <summary>
    /// Опыт (в годах)
    /// </summary>
    public int Experience { get; set; } = 0;

    public LookingFor LookingFor { get; set; } = LookingFor.NotLooking;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // === Навигационные свойства ===
    public City City { get; set; } = null!;
    public string Email { get; set; } = string.Empty;

    // === Коллекции ===
    public List<Genre> Genres { get; set; } = new();
    public List<MusicalSpecialty> Specialties { get; set; } = new();
    public List<CollaborationGoal> CollaborationGoals { get; set; } = new();

    public List<Genre> DesiredGenres { get; set; } = new();
    public List<MusicalSpecialty> DesiredSpecialties { get; set; } = new();

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