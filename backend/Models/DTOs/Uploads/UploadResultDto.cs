namespace backend.Models.DTOs.Uploads
{
    public class UploadResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
