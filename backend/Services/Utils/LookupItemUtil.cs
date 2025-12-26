using backend.Models.DTOs;

namespace backend.Services.Utils
{
    public static class LookupItemUtil
    {
        public static LookupItemDto ToLookupItem(ILookupItem item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            LocalizedName = item.LocalizedName
        };
    }
}
