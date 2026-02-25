using backend.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Profiles
{
    public class UpdateProfileRequest
    {
        public ProfileType? ProfileType { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; } = string.Empty;

        [Range(0, 100)]
        public int? Age { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? Telegram { get; set; }

        public int? CityId { get; set; }

        [Range(0, int.MaxValue)]
        public int? Experience { get; set; }

        public LookingFor? LookingFor { get; set; }

        public List<int>? DesiredGenreIds { get; set; }
        public List<int>? DesiredSpecialtyIds { get; set; }

        public List<int>? GenreIds { get; set; }
        public List<int>? SpecialtyIds { get; set; }
        public List<int>? CollaborationGoalIds { get; set; }
    }
}