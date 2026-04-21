using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Profiles.UpdateAvatar
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateAvatarCommand"/>.
    /// </summary>
    public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, string>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateAvatarCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="fileStorage">Сервис файлового хранилища.</param>
        public UpdateAvatarCommandHandler(
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            if (!request.ContentType.StartsWith("image/"))
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure(nameof(request.ContentType), "Разрешены только изображения.") });

            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

            if (!string.IsNullOrEmpty(profile.AvatarUrl))
                await _fileStorage.DeleteFileAsync(profile.AvatarUrl);

            var fileUrl = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType);
            profile.SetAvatar(fileUrl);

            await _profileRepository.UpdateAsync(profile);

            return fileUrl;
        }
    }
}