using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Events
{
    public class CreateEventRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int MaxParticipants { get; set; }
    }
}
