namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Город (справочник).
    /// </summary>
    public class City
    {
        private City()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр города.
        /// </summary>
        /// <param name="id">Уникальный идентификатор.</param>
        /// <param name="name">Английское название.</param>
        /// <param name="localizedName">Локализованное название.</param>
        public City(int id, string name, string localizedName)
        {
            Id = id;
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название.
        /// </summary>
        public string LocalizedName { get; private set; }
    }
}