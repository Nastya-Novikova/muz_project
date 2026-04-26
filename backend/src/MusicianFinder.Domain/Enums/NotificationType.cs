namespace MusicianFinder.Domain.Enums
{
    /// <summary>
    /// Тип уведомления.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Получено предложение о сотрудничестве.
        /// </summary>
        CollaborationReceived,

        /// <summary>
        /// Регистрация на мероприятие.
        /// </summary>
        EventRegistration,

        /// <summary>
        /// Напоминание о мероприятии.
        /// </summary>
        EventReminder
    }
}