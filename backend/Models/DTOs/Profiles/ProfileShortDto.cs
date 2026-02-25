namespace backend.Models.DTOs.Profiles
{
    public class ProfileShortDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? CityName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
