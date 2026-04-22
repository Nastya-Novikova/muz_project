using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Обработчик команды <see cref="RemoveFavoriteCommand"/>.
    /// </summary>
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RemoveFavoriteCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public RemoveFavoriteCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _dbContext.Users
                .Include(nameof(Domain.Entities.User.Favorites))
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            user.RemoveFavorite(request.ProfileId);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}