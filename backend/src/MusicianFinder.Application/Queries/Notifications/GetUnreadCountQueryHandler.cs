using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Обработчик запроса <see cref="GetUnreadCountQuery"/>.
    /// </summary>
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetUnreadCountQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public GetUnreadCountQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            return await _dbContext.Notifications
                .Where(n => n.ProfileId == profile.Id && !n.IsRead)
                .CountAsync(cancellationToken);
        }
    }
}