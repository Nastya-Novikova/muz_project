using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<(List<Notification> Items, int TotalCount)> GetByProfileIdAsync(Guid profileId, int page, int limit, DateTime? fromDate = null);
        Task<Notification?> GetByIdAsync(Guid id);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(Guid profileId);
        Task<int> GetUnreadCountAsync(Guid profileId);
    }
}
