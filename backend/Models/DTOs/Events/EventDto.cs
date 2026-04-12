using backend.Models.Enums;

namespace backend.Models.DTOs.Events
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public LookupItemDto Region { get; set; } = new();
        public LookupItemDto City { get; set; } = new();
        public string Address { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public bool IsRegistered { get; set; } // для текущего пользователя

        public EventStatus Status { get; set; }

        public Guid CreatorProfileId { get; set; }
        public string CreatorFullName { get; set; } = string.Empty;
        public string? CreatorAvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
