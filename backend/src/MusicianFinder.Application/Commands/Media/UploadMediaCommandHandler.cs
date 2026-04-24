using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Обработчик команды <see cref="UploadMediaCommand"/>.
    /// </summary>
    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, Guid>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadMediaCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public UploadMediaCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .Include(nameof(MusicianProfile.PortfolioItems))
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            using var stream = new MemoryStream(request.Content);
            var fileUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);

            var portfolioItem = new PortfolioItem(
                request.Title,
                fileUrl,
                request.ContentType,
                request.Type,
                null)
            {
                Description = request.Description
            };

            profile.AddPortfolioItem(portfolioItem);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return portfolioItem.Id;
        }
    }
}