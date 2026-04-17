using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с уведомлениями.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public NotificationRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        /// <inheritdoc />
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.MarkAsRead();
                _context.Notifications.Update(notification);
            }
        }

        /// <inheritdoc />
        public async Task MarkAllAsReadAsync(Guid profileId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.ProfileId == profileId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unreadNotifications)
                n.MarkAsRead();

            _context.Notifications.UpdateRange(unreadNotifications);
        }

        /// <inheritdoc />
        public async Task<int> GetUnreadCountAsync(Guid profileId)
        {
            return await _context.Notifications
                .CountAsync(n => n.ProfileId == profileId && !n.IsRead);
        }
    }
}