using backend.Models.Enums;

namespace backend.Models.DTOs.Events
{
    public class EventFilterRequest
    {
        public string? Query { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public EventStatus? Status { get; set; }
        public Guid? CreatorProfileId { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = true;
    }
}
