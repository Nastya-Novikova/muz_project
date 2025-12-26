namespace backend.Models.DTOs
{
    public interface ILookupItem
    {
        int Id { get; set; }
        string Name { get; set; }
        string LocalizedName { get; set; }
    }
}
