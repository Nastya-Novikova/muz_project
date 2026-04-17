using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Цель сотрудничества.
    /// </summary>
    public class CollaborationGoal
    {
        private readonly List<MusicianProfile> _profiles = new();

        private CollaborationGoal() { }

        public CollaborationGoal(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название цели.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название цели.
        /// </summary>
        public string LocalizedName { get; private set; }

        /// <summary>
        /// Связанные профили.
        /// </summary>
        public IReadOnlyCollection<MusicianProfile> Profiles => _profiles.AsReadOnly();
    }
}
