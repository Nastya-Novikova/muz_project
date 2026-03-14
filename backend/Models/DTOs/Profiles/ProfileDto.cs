using backend.Models.Enums;
using System;
using System.Collections.Generic;

namespace backend.Models.DTOs.Profiles
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public ProfileType ProfileType { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Telegram { get; set; }
        public LookupItemDto City { get; set; } = new();
        public int Experience { get; set; }
        public LookingFor LookingFor { get; set; }
        public string Email { get; set; } = string.Empty;

        public List<LookupItemDto> DesiredGenres { get; set; } = new();
        public List<LookupItemDto> DesiredSpecialties { get; set; } = new();

        public List<LookupItemDto> Genres { get; set; } = new();
        public List<LookupItemDto> Specialties { get; set; } = new();
        public List<LookupItemDto> CollaborationGoals { get; set; } = new();
    }
}