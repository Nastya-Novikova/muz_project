using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Events
{
    public class UpdateEventRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? MaxParticipants { get; set; }
    }
}
