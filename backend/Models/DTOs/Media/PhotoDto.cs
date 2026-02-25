namespace backend.Models.DTOs.Media
{
    public class PhotoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
