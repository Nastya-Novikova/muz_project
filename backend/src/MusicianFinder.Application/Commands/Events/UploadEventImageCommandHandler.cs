using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using ValidationException = MusicianFinder.Application.Common.Exceptions.ValidationException;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UploadEventImageCommand"/>.
    /// </summary>
    public class UploadEventImageCommandHandler : IRequestHandler<UploadEventImageCommand, string>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadEventImageCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public UploadEventImageCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IFileStorage fileStorage)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            if (!request.ContentType.StartsWith("image/"))
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.ContentType), "Разрешены только изображения.") });

            if (request.FileStream.Length > 5 * 1024 * 1024)
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.FileStream), "Файл слишком большой (макс. 5 МБ).") });

            var eventEntity = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            if (eventEntity.CreatorProfileId != profile.Id)
                throw new ForbiddenException("Только создатель может загружать изображение.");

            if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
                await _fileStorage.DeleteFileAsync(eventEntity.ImageUrl);

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);
            eventEntity.SetImage(fileUrl, profile.Id);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return fileUrl;
        }
    }
}