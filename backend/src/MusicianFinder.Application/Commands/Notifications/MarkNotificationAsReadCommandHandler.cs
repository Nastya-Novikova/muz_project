using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Notifications
{
    /// <summary>
    /// Обработчик команды <see cref="MarkNotificationAsReadCommand"/>.
    /// </summary>
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MarkNotificationAsReadCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public MarkNotificationAsReadCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken)
                ?? throw new NotFoundException(nameof(Notification), request.NotificationId);

            if (notification.ProfileId != profile.Id)
                throw new ForbiddenException("Нет доступа к этому уведомлению.");

            notification.MarkAsRead();
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}