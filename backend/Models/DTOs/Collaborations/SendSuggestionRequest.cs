namespace backend.Models.DTOs.Collaborations
{
    public class SendSuggestionRequest
    {
        public Guid ToProfileId { get; set; }
        public string? Message { get; set; }
    }
}
