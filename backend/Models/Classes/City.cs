using backend.Models.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

/// <summary>
/// Справочник городов
/// </summary>
[Table("Cities")]
public class City
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Английское название города
    /// </summary>
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название города
    /// </summary>
    [Required, MaxLength(50)]
    public string LocalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Связанные профили
    /// </summary>
    public List<MusicianProfile> Profiles { get; set; } = new();
}