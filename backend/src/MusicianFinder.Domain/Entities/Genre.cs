using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальный жанр.
    /// </summary>
    public class Genre
    {
        private readonly List<MusicianProfile> _profiles = new();
        private readonly List<MusicianProfile> _profilesLookingForThisGenre = new();

        private Genre() { }

        public Genre(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
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
