namespace backend.Models.DTOs
{
    public record LookupItemDto : ILookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocalizedName { get; set; } = string.Empty;
    }
}
