using System.Collections.Generic;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Цель сотрудничества.
    /// </summary>
    public class CollaborationGoal
    {
        private readonly List<MusicianProfile> _profiles = [];

        private CollaborationGoal()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр цели сотрудничества.
        /// </summary>
        /// <param name="name">Английское название цели.</param>
        /// <param name="localizedName">Русское название цели.</param>
        public CollaborationGoal(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор цели.
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