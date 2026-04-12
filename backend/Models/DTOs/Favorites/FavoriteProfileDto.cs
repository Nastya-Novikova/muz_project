using backend.Models.DTOs.Profiles;
using System;
using System.Collections.Generic;

namespace backend.Models.DTOs.Favorites
{
    public class FavoriteProfileDto
    {
        public ProfileDto Profile { get; set; } = new();
        public DateTime AddedAt { get; set; }
    }
}