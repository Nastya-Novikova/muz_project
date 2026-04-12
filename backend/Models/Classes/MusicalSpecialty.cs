using backend.Models.DTOs;

namespace backend.Models.Classes;

/// <summary>
/// Музыкальная специальность: вокалист, гитарист, композитор и т.д.
/// </summary>
public class MusicalSpecialty : ILookupItem
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Английское название специальности
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название специальности
    /// </summary>
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();

    // Профили, которые ищут эту специализацию
    public List<MusicianProfile> ProfilesLookingForThisSpecialty { get; set; } = new();
}