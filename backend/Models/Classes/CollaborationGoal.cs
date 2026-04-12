using backend.Models.DTOs;

namespace backend.Models.Classes;

/// <summary>
/// Цель сотрудничества
/// </summary>
public class CollaborationGoal : ILookupItem
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Английское название цели
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название цели
    /// </summary>
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();
}