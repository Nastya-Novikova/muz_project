using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для уведомлений.
    /// </summary>
    public class NotificationReadRepository : INotificationReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationReadRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public NotificationReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<NotificationDto>> GetForProfileAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .SelectMany(p => p.Notifications)
                .OrderByDescending(n => n.CreatedAt);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * limit).Take(limit)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<NotificationDto> { Items = items, Total = total, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<int> GetUnreadCountAsync(Guid profileId, CancellationToken ct)
        {
            return await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .SelectMany(p => p.Notifications)
                .Where(n => !n.IsRead)
                .CountAsync(ct);
        }
    }
}