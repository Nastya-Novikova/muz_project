namespace MusicianFinder.Domain.Common
{
    /// <summary>
    /// Интерфейс для сущностей, поддерживающих мягкое удаление.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Признак того, что сущность помечена как удалённая.
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Дата и время мягкого удаления.
        /// </summary>
        DateTime? DeletedAt { get; }

        /// <summary>
        /// Помечает сущность как удалённую.
        /// </summary>
        void MarkAsDeleted();
    }
}