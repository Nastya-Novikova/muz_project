using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="DeleteMediaCommand"/>.
    /// </summary>
    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeleteMediaCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public DeleteMediaCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IFileStorage fileStorage)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .Include(nameof(Domain.Entities.MusicianProfile.PortfolioItems))
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var mediaItem = profile.PortfolioItems.FirstOrDefault(p => p.Id == request.MediaId)
                ?? throw new NotFoundException("Медиа не найдено.");

            await _fileStorage.DeleteFileAsync(mediaItem.FileUrl);
            profile.RemovePortfolioItem(request.MediaId);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}