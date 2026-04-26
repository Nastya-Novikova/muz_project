namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Цель сотрудничества (справочник).
    /// </summary>
    public class CollaborationGoal
    {
        private CollaborationGoal()
        {
            Name = string.Empty;
            LocalizedName = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр цели сотрудничества.
        /// </summary>
        /// <param name="id">Уникальный идентификатор.</param>
        /// <param name="name">Английское название цели.</param>
        /// <param name="localizedName">Русское название цели.</param>
        public CollaborationGoal(int id, string name, string localizedName)
        {
            Id = id;
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
    }
}