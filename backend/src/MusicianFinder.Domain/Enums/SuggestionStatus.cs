namespace MusicianFinder.Domain.Enums
{
    /// <summary>
    /// Статус предложения о сотрудничестве.
    /// </summary>
    public enum SuggestionStatus
    {
        /// <summary>
        /// Ожидает рассмотрения.
        /// </summary>
        Pending,

        /// <summary>
        /// Принято.
        /// </summary>
        Accepted,

        /// <summary>
        /// Отклонено.
        /// </summary>
        Rejected,

        /// <summary>
        /// Отозвано отправителем.
        /// </summary>
        Withdrawn
    }
}