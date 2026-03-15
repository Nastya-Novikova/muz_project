namespace backend.Models.DTOs.Profiles
{
    public class ProfileShortDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public LookupItemDto City { get; set; } = new();
        public string? AvatarUrl { get; set; }
        public List<LookupItemDto> Genres { get; set; } = new();
        public List<LookupItemDto> Specialties { get; set; } = new();
        public string? Description { get; set; }

        //Description
        //Genres
        //Specialities

    }
}
