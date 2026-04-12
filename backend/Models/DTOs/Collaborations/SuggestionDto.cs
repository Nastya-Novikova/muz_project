using backend.Models.DTOs.Profiles;

namespace backend.Models.DTOs.Collaborations
{
    public class SuggestionDto
    {
        public Guid Id { get; set; }
        public ProfileDto FromProfile { get; set; } = new();
        public ProfileDto ToProfile { get; set; } = new();
        public string? Message { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
