namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Справочник регионов.
    /// </summary>
    public class Region
    {
        private Region()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр региона.
        /// </summary>
        /// <param name="name">Английское название региона.</param>
        /// <param name="localizedName">Русское название региона.</param>
        public Region(string name, string localizedName)
        {
            Name = name;
            LocalizedName = localizedName;
        }

        /// <summary>
        /// Идентификатор региона.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Английское название региона.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Русское название региона.
        /// </summary>
        public string LocalizedName { get; private set; }
    }
}