using backend.Models.Classes;

namespace backend.Models.Classes;

public class PortfolioPhoto
{
    public Guid Id { get; set; }

    public Guid ProfileId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FileUrl { get; set; }
    
    public string MimeType { get; set; } = "image/jpeg";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MusicianProfile Profile { get; set; } = null!;
}