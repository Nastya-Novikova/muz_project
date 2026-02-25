namespace backend.Models.Classes
{
    public class Favorite
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid ProfileId { get; set; }
        public MusicianProfile Profile { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
