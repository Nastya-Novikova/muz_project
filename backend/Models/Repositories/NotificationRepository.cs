using backend.Data;
using backend.Exceptions;
using backend.Models.Repositories.Interfaces;
using backend.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly MusicianFinderDbContext _context;

        public NotificationRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Notification> Items, int TotalCount)> GetByProfileIdAsync(Guid profileId, int page, int limit, DateTime? fromDate = null)
        {
            var query = _context.Notifications.Where(n => n.ProfileId == profileId);

            if (fromDate.HasValue)
                query = query.Where(n => n.CreatedAt >= fromDate.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task AddAsync(Notification notification)
        {
            if (notification == null)
                throw new ApiException(400, "Уведомление не может быть null", "NOTIFICATION_IS_NULL");

            await _context.Notifications.AddAsync(notification);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                throw new ApiException(404, "Уведомление не найдено", "NOTIFICATION_NOT_FOUND");

            notification.IsRead = true;
            _context.Notifications.Update(notification);
        }

        public async Task MarkAllAsReadAsync(Guid profileId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.ProfileId == profileId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unreadNotifications)
                n.IsRead = true;

            _context.Notifications.UpdateRange(unreadNotifications);
        }

        public async Task<int> GetUnreadCountAsync(Guid profileId)
        {
            return await _context.Notifications
                .CountAsync(n => n.ProfileId == profileId && !n.IsRead);
        }
    }
}
