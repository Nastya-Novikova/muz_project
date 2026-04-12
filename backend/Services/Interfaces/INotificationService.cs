using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Notifications;
using backend.Models.Enums;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с уведомлениями
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Создать уведомление о получении предложения о сотрудничестве
        /// </summary>
        //Task CreateCollaborationReceivedAsync(Guid recipientProfileId, Guid suggestionId, string fromProfileName);

        /// <summary>
        /// Создать уведомление о записи на мероприятие (для создателя мероприятия)
        /// </summary>
        //Task CreateEventRegistrationAsync(Guid eventCreatorProfileId, Guid eventId, string eventTitle, Guid registeredProfileId);

        /// <summary>
        /// Создать уведомление-напоминание о предстоящем мероприятии
        /// </summary>
        //Task CreateEventReminderAsync(Guid profileId, Guid eventId, string eventTitle, int daysLeft);

        /// <summary>
        /// Получить уведомления пользователя за последние 30 дней
        /// </summary>
        Task<Result<PagedResult<NotificationDto>>> GetUserNotificationsAsync(Guid userId, int page, int limit);

        /// <summary>
        /// Отметить уведомление как прочитанное
        /// </summary>
        Task<Result> MarkAsReadAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Отметить все уведомления пользователя как прочитанные
        /// </summary>
        Task<Result> MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Получить количество непрочитанных уведомлений
        /// </summary>
        Task<int> GetUnreadCountAsync(Guid userId);

        /// <summary>
        /// Отправить уведомление профилю с учётом его настроек (Email, VK, внутреннее)
        /// </summary>
        Task SendNotificationToProfileAsync(Guid profileId, NotificationType type, Dictionary<string, object> data);

        /// <summary>
        /// Отправить уведомление пользователю (по userId) с учётом настроек
        /// </summary>
        Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data);
    }
}
