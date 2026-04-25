namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Музыкальная специальность (справочник).
    /// </summary>
    public class MusicalSpecialty
    {
        private MusicalSpecialty()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр специальности.
        /// </summary>
        /// <param name="id">Уникальный идентификатор.</param>
        /// <param name="name">Английское название специальности.</param>
        /// <param name="localizedName">Русское название специальности.</param>
        public MusicalSpecialty(int id, string name, string localizedName)
        {
            Id = id;
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
    }
}