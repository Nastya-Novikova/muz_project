using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Обработчик команды <see cref="MarkAllNotificationsAsReadCommand"/>.
    /// </summary>
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MarkAllNotificationsAsReadCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public MarkAllNotificationsAsReadCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var unreadNotifications = await _dbContext.Notifications
                .Where(n => n.ProfileId == profile.Id && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var n in unreadNotifications)
                n.MarkAsRead();

            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}