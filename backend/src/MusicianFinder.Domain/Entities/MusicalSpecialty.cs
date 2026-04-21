using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальная специальность (вокалист, гитарист, композитор и т.д.).
    /// </summary>
    public class MusicalSpecialty
    {
        private readonly List<MusicianProfile> _profiles = new();
        private readonly List<MusicianProfile> _profilesLookingForThisSpecialty = new();

        private MusicalSpecialty() { }

        public MusicalSpecialty(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
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
