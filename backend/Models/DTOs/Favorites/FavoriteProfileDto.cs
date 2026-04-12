using backend.Models.DTOs.Profiles;
using System;
using System.Collections.Generic;

namespace backend.Models.DTOs.Favorites
{
    public class FavoriteProfileDto
    {
        /*public Guid ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public LookupItemDto City { get; set; } = new();
        public string? AvatarUrl { get; set; }
        public List<LookupItemDto> Genres { get; set; } = new();
        public List<LookupItemDto> Specialties { get; set; } = new();
        public string? Description { get; set; }*/
        public ProfileDto Profile { get; set; } = new();
        public DateTime AddedAt { get; set; }
    }
}