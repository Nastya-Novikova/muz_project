namespace MusicianFinder.Application.DTOs.Metadata
{
    /// <summary>
    /// DTO элемента справочника (город, жанр, специальность и т.п.).
    /// </summary>
    public class LookupItemDto
    {
        /// <summary>Идентификатор.</summary>
        public Guid Id { get; set; }
        /// <summary>Английское название.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Локализованное название.</summary>
        public string LocalizedName { get; set; } = string.Empty;
    }
}