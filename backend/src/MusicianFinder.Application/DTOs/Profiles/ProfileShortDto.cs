using MusicianFinder.Application.DTOs.Metadata;

namespace MusicianFinder.Application.DTOs.Profiles
{
    /// <summary>
    /// Краткий DTO профиля.
    /// </summary>
    public class ProfileShortDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Полное имя.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Город.
        /// </summary>
        public LookupItemDto City { get; set; } = new();

        /// <summary>
        /// URL аватара.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Жанры.
        /// </summary>
        public List<LookupItemDto> Genres { get; set; } = new();

        /// <summary>
        /// Специальности.
        /// </summary>
        public List<LookupItemDto> Specialties { get; set; } = new();

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }
    }
}