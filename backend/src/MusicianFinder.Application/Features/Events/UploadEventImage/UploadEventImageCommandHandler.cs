using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Features.Events.UploadEventImage
{
    /// <summary>
    /// Обработчик команды <see cref="UploadEventImageCommand"/>.
    /// </summary>
    public class UploadEventImageCommandHandler : IRequestHandler<UploadEventImageCommand, string>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadEventImageCommandHandler"/>.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public UploadEventImageCommandHandler(
            IEventRepository eventRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _eventRepository = eventRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            if (!request.ContentType.StartsWith("image/"))
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.ContentType), "Разрешены только изображения.") });

            if (request.FileStream.Length > 5 * 1024 * 1024)
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.FileStream), "Файл слишком большой (макс. 5 МБ).") });

            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                throw new NotFoundException(nameof(Event), request.EventId);

            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            if (eventEntity.CreatorProfileId != profile.Id)
                throw new ForbiddenException("Только создатель может загружать изображение.");

            if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
                await _fileStorage.DeleteFileAsync(eventEntity.ImageUrl);

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);
            eventEntity.SetImage(fileUrl, profile.Id);

            await _eventRepository.UpdateAsync(eventEntity);

            return fileUrl;
        }
    }
}