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
