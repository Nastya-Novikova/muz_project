using backend.Models.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

[Table("PortfolioPhotos")]
public class PortfolioPhoto
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProfileId { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public byte[] FileData { get; set; } = Array.Empty<byte>();
    [MaxLength(50)]
    public string MimeType { get; set; } = "image/jpeg";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProfileId")]
    public MusicianProfile Profile { get; set; } = null!;
}