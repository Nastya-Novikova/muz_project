using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("MusicianProfiles")]
public class MusicianProfile
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LookingFor { get; set; } = "collaboration";

    [Column(TypeName = "text[]")]
    public List<string> Instruments { get; set; } = new();

    [Column(TypeName = "text[]")]
    public List<string> Genres { get; set; } = new();
}