using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models.DTOs;

namespace backend.Models.Classes;

/// <summary>
/// Цель сотрудничества
/// </summary>
[Table("CollaborationGoals")]
public class CollaborationGoal : ILookupItem
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Английское название цели
    /// </summary>
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название цели
    /// </summary>
    [Required, MaxLength(50)]
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();
}