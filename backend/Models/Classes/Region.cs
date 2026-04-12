using backend.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace backend.Models.Classes
{
    /// <summary>
    /// Справочник регионов
    /// </summary>
    public class Region : ILookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocalizedName { get; set; } = string.Empty;
    }
}
