using backend.Models.Enums;

namespace backend.Models.Classes
{
    /// <summary>
    /// Мероприятие
    /// </summary>
    public class Event : ISoftDeletable
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public string Address { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public int MaxParticipants { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Scheduled;

        public Guid CreatorProfileId { get; set; }
        public MusicianProfile CreatorProfile { get; set; } = null!;

        public List<EventRegistration> Registrations { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ISoftDeletable
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
