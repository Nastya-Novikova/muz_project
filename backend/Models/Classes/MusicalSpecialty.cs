using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Музыкальная специальность: вокалист, гитарист, композитор и т.д.
/// </summary>
[Table("MusicalSpecializations")]
public class MusicalSpecialty
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Английское название специальности
    /// </summary>
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название специальности
    /// </summary>
    [Required, MaxLength(50)]
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();
}