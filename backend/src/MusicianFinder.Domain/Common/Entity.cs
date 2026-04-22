using System;

namespace MusicianFinder.Domain.Common
{
    /// <summary>
    /// Базовый класс для сущностей с идентификатором.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Уникальный идентификатор сущности.
        /// </summary>
        public Guid Id { get; protected set; }
    }
}