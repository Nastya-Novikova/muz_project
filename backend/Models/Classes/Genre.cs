using backend.Models.Classes;
using backend.Models.DTOs;

namespace backend.Models.Classes;

/// <summary>
/// Музыкальный жанр
/// </summary>
public class Genre : ILookupItem
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Английское название жанра
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название жанра
    /// </summary>
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();

    // Профили, которые ищут этот жанр
    public List<MusicianProfile> ProfilesLookingForThisGenre { get; set; } = new();
}