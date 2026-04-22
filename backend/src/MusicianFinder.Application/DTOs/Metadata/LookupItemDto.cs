namespace MusicianFinder.Application.DTOs.Metadata
{
    /// <summary>
    /// DTO элемента справочника.
    /// </summary>
    public class LookupItemDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название на английском.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Локализованное название.
        /// </summary>
        public string LocalizedName { get; set; } = string.Empty;
    }
}