namespace backend.Models.Classes
{
    /// <summary>
    /// Связь многие-ко-многим между мероприятием и профилем музыканта (участники)
    /// </summary>
    public class EventRegistration
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;

        public Guid ProfileId { get; set; }
        public MusicianProfile Profile { get; set; } = null!;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
