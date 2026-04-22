namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Справочник городов.
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
        /// <param name="name">Английское название.</param>
        /// <param name="localizedName">Локализованное название.</param>
        public City(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор.
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