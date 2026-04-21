using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Uploads.DeletePhoto
{
    /// <summary>
    /// Обработчик команды <see cref="DeletePhotoCommand"/>.
    /// </summary>
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, Unit>
    {
        private readonly IPortfolioPhotoRepository _photoRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorage _fileStorage;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DeletePhotoCommandHandler"/>.
        /// </summary>
        public DeletePhotoCommandHandler(
            IPortfolioPhotoRepository photoRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IFileStorage fileStorage)
        {
            _photoRepository = photoRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var photo = await _photoRepository.GetByIdAsync(request.Id);
            if (photo == null)
                throw new NotFoundException("Фото не найдено.");

            if (photo.ProfileId != profile.Id)
                throw new ForbiddenException("Нет прав на удаление этого фото.");

            await _fileStorage.DeleteFileAsync(photo.FileUrl);

            await _photoRepository.RemoveAsync(request.Id);

            return Unit.Value;
        }
    }
}