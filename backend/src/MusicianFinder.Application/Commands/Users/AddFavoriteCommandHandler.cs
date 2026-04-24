using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Users
{
    /// <summary>
    /// Обработчик команды <see cref="AddFavoriteCommand"/>.
    /// </summary>
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AddFavoriteCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public AddFavoriteCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _dbContext.Users
                .Include(nameof(Domain.Entities.User.Favorites))
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            user.AddFavorite(request.ProfileId);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}