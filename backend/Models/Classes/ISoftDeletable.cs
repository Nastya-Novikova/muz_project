namespace backend.Models.Classes
{
    /// <summary>
    /// Интерфейс для сущностей с soft-delete
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Флаг удаления
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Время удаления
        /// </summary>
        DateTime? DeletedAt { get; set; }
    }
}
