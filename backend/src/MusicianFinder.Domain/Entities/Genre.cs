namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальный жанр (справочник).
    /// </summary>
    public class Genre
    {
        private Genre()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр жанра.
        /// </summary>
        /// <param name="id">Уникальный идентификатор.</param>
        /// <param name="name">Английское название жанра.</param>
        /// <param name="localizedName">Русское название жанра.</param>
        public Genre(Guid id, string name, string localizedName)
        {
            Id = id;
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор жанра.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Английское название жанра.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название жанра.
        /// </summary>
        public string LocalizedName { get; private set; }
    }
}