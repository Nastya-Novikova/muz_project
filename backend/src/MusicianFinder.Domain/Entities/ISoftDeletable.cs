using System;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Интерфейс для сущностей, поддерживающих мягкое удаление.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Признак того, что сущность удалена (мягкое удаление).
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Дата и время мягкого удаления.
        /// </summary>
        DateTime? DeletedAt { get; }

        /// <summary>
        /// Пометить сущность как удалённую.
        /// </summary>
        void MarkAsDeleted();
    }
}