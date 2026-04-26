namespace MusicianFinder.Domain.Enums
{
    /// <summary>
    /// Статус мероприятия.
    /// </summary>
    public enum EventStatus
    {
        /// <summary>
        /// Запланировано.
        /// </summary>
        Scheduled,

        /// <summary>
        /// Отменено.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Завершено.
        /// </summary>
        Completed
    }
}