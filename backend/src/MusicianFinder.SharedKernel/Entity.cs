namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Базовый класс для всех сущностей предметной области.
    /// Содержит уникальный идентификатор.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Уникальный идентификатор сущности.
        /// </summary>
        public Guid Id { get; protected set; }
    }
}