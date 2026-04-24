using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Обработчик запроса <see cref="GetNotificationsQuery"/>.
    /// </summary>
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetNotificationsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetNotificationsQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var query = _dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.ProfileId == profile.Id && n.CreatedAt >= thirtyDaysAgo);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PagedResult<NotificationDto>
            {
                Items = items,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}