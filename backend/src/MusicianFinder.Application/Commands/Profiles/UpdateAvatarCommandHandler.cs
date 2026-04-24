using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using ValidationException = MusicianFinder.Application.Core.Exceptions.ValidationException;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateAvatarCommand"/>.
    /// </summary>
    public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, string>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateAvatarCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public UpdateAvatarCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            if (!request.ContentType.StartsWith("image/"))
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.ContentType), "Разрешены только изображения.") });

            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            if (!string.IsNullOrEmpty(profile.AvatarUrl))
                await _fileStorage.DeleteFileAsync(profile.AvatarUrl);

            using var stream = new MemoryStream(request.Content);
            var fileUrl = await _fileStorage.SaveFileAsync(stream, request.FileName, request.ContentType);
            profile.SetAvatar(fileUrl);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return fileUrl;
        }
    }
}