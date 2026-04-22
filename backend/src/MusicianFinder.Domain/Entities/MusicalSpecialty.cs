using System.Collections.Generic;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальная специальность.
    /// </summary>
    public class MusicalSpecialty
    {
        private readonly List<MusicianProfile> _profiles = [];
        private readonly List<MusicianProfile> _profilesLookingForThisSpecialty = [];

        private MusicalSpecialty()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр специальности.
        /// </summary>
        /// <param name="name">Английское название специальности.</param>
        /// <param name="localizedName">Русское название специальности.</param>
        public MusicalSpecialty(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор специальности.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название специальности.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название специальности.
        /// </summary>
        public string LocalizedName { get; private set; }

        /// <summary>
        /// Профили, предлагающие эту специальность.
        /// </summary>
        public IReadOnlyCollection<MusicianProfile> Profiles => _profiles.AsReadOnly();

        /// <summary>
        /// Профили, которые ищут эту специальность.
        /// </summary>
        public IReadOnlyCollection<MusicianProfile> ProfilesLookingForThisSpecialty => _profilesLookingForThisSpecialty.AsReadOnly();
    }
}