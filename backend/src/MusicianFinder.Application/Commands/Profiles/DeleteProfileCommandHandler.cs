using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteProfileCommand"/>.
    /// </summary>
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeleteProfileCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public DeleteProfileCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(nameof(User.MusicianProfile))
                .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId && !u.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

            if (user.MusicianProfile == null)
                throw new NotFoundException("Профиль не найден.");

            user.MusicianProfile.MarkAsDeleted();
            user.ClearMusicianProfile();
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}