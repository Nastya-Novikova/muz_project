using System.Collections.Generic;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальный жанр.
    /// </summary>
    public class Genre
    {
        private readonly List<MusicianProfile> _profiles = [];
        private readonly List<MusicianProfile> _profilesLookingForThisGenre = [];

        private Genre()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр жанра.
        /// </summary>
        /// <param name="name">Английское название жанра.</param>
        /// <param name="localizedName">Русское название жанра.</param>
        public Genre(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор жанра.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название жанра.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название жанра.
        /// </summary>
        public string LocalizedName { get; private set; }

        /// <summary>
        /// Профили, предлагающие этот жанр.
        /// </summary>
        public IReadOnlyCollection<MusicianProfile> Profiles => _profiles.AsReadOnly();

        /// <summary>
        /// Профили, которые ищут этот жанр.
        /// </summary>
        public IReadOnlyCollection<MusicianProfile> ProfilesLookingForThisGenre => _profilesLookingForThisGenre.AsReadOnly();
    }
}