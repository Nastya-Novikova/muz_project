using backend.Models.Enums;
using System.Collections.Generic;

namespace backend.Models.DTOs.Profiles
{
    public class SearchRequest
    {
        public string? Query { get; set; }
        public int? CityId { get; set; }
        public List<int>? GenreIds { get; set; }
        public List<int>? SpecialtyIds { get; set; }
        public List<int>? GoalIds { get; set; }
        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }

        public LookingFor? LookingFor { get; set; }
        public ProfileType? ProfileType { get; set; }
        public List<int>? DesiredGenreIds { get; set; }
        public List<int>? DesiredSpecialtyIds { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = true;
    }
}